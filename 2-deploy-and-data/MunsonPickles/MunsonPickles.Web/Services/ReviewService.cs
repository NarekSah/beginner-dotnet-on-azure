using MunsonPickles.Web.Data;
using MunsonPickles.Web.Models;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using MunsonPickles.Events;

namespace MunsonPickles.Web.Services;

public class ReviewService
{
	private readonly PickleDbContext _pickleContext;
    private readonly IEventGridPublisher _eventGridPublisher;
    private readonly LoggingService _loggingService;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(
        PickleDbContext context, 
        IEventGridPublisher eventGridPublisher,
        LoggingService loggingService,
        ILogger<ReviewService> logger)
	{
		_pickleContext = context;
        _eventGridPublisher = eventGridPublisher;
        _loggingService = loggingService;
        _logger = logger;
    }

	public async Task AddReview(string reviewText, int productId)
	{
        string userId = "matt"; // this will get changed out when we add auth
        
        _logger.LogInformation("Adding review for product {ProductId} by user {UserId}", productId, userId);

        try
		{
            await _loggingService.LogExecutionTimeAsync("AddReview", async () => 
            {
                // create the new review
                Review review = new()
                {
                    Date = DateTime.Now,                
                    Text = reviewText,
                    UserId = userId
                };

                _logger.LogDebug("Looking up product with ID {ProductId}", productId);
                Product? product = await _pickleContext.Products.FindAsync(productId);

                if (product is null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", productId);
                    return false;
                }

                if (product.Reviews is null)
                {
                    _logger.LogDebug("Initializing review collection for product {ProductId}", productId);
                    product.Reviews = new List<Review>();
                }

                product.Reviews.Add(review);
                
                _logger.LogDebug("Saving review to database");
                await _pickleContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully saved review {ReviewId} for product {ProductId}", 
                    review.Id, productId);

                var reviewEvent = new MunsonPickles.Events.ReviewEvent
                {
                    Id = review.Id,
                    ProductId = productId,
                    HasPhotos = review.Photos?.Any() ?? false,
                    UserId = userId
                };

                _logger.LogDebug("Publishing event for review {ReviewId}", review.Id);
                await _eventGridPublisher.PublishEventAsync(reviewEvent);
                _logger.LogInformation("Event published for review {ReviewId}", review.Id);
                
                return true;
            });
        }
		catch (Exception ex)
		{
            _logger.LogError(ex, "Error adding review for product {ProductId}: {ErrorMessage}", 
                productId, ex.Message);
		}		
	}

	public async Task<IEnumerable<Review>> GetReviewsForProduct(int productId)
	{
        _logger.LogInformation("Getting reviews for product {ProductId}", productId);
        
        return await _loggingService.LogExecutionTimeAsync("GetReviewsForProduct", async () => 
        {
            var reviews = await _pickleContext.Reviews
                .AsNoTracking()
                .Where(r => r.Product.Id == productId)
                .ToListAsync();
                
            _logger.LogInformation("Retrieved {ReviewCount} reviews for product {ProductId}", 
                reviews.Count, productId);
                
            return reviews;
        });
	}
}
