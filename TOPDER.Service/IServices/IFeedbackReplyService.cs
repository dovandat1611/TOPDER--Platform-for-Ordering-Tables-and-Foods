using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Dtos.FeedbackReply;

namespace TOPDER.Service.IServices
{
    public interface IFeedbackReplyService 
    {
        Task<FeedbackDto> AddAsync(CreateFeedbackReplyDto feedbackReplyDto);
        Task<bool> InvisibleAsync(int id);
    }
}
