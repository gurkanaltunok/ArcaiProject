using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.Helpers;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Entities;
using ArcaiProject.Entities.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ArcaiProject.Business.Services
{
    public class BorrowingRecordService : IBorrowingRecordService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public BorrowingRecordService(ArcaiDbContext context, IMapper mapper, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<BorrowingRecordDto> RequestDocumentAsync(CreateBorrowingRequestDto createDto, int professorId)
        {
            var document = await _context.Documents.FindAsync(createDto.DocumentId);
            if (document == null || document.IsDeleted)
            {
                throw new InvalidOperationException("Document not found.");
            }
            if (document.Status != DocumentStatus.Available)
            {
                var statusMessage = document.Status switch
                {
                    DocumentStatus.CheckedOut => "This document is currently checked out and cannot be requested until it is returned.",
                    DocumentStatus.Missing => "This document is marked as missing and cannot be requested.",
                    _ => "This document is not available for borrowing."
                };
                throw new InvalidOperationException(statusMessage);
            }

            var record = _mapper.Map<BorrowingRecord>(createDto);
            record.RequesterUserId = professorId;
            record.Status = BorrowingRecordStatus.Pending;
            record.RequestDate = DateTime.UtcNow;

            _context.BorrowingRecords.Add(record);
            await _context.SaveChangesAsync();

            // Notify admin (first active Admin user)
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin" && u.IsActive == true);
            if (adminUser != null)
            {
                await _notificationService.CreateNotificationAsync(adminUser.Id, $"A new document request (Document ID: {record.DocumentId}) is awaiting your approval.", $"/borrowing/{record.Id}");
            }

            var loaded = await _context.BorrowingRecords.Where(r => r.Id == record.Id).IncludeFullDetails().AsNoTracking().FirstAsync();
            return _mapper.Map<BorrowingRecordDto>(loaded);
        }

        public async Task<PagedList<BorrowingRecordDto>> GetMyRequestsAsync(int professorId, PagingParameters pagingParameters)
        {
            var query = _context.BorrowingRecords
                .Where(r => r.RequesterUserId == professorId)
                .IncludeFullDetails()
                .OrderByDescending(r => r.RequestDate)
                .AsNoTracking();

            var dtoQuery = query.ProjectTo<BorrowingRecordDto>(_mapper.ConfigurationProvider);

            return await PagedList<BorrowingRecordDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<BorrowingRecordDto?> ApproveRequestAsync(int recordId, int adminId)
        {
            var record = await _context.BorrowingRecords
                .Include(r => r.RequesterUser)
                .FirstOrDefaultAsync(r => r.Id == recordId);
            if (record == null || record.Status != BorrowingRecordStatus.Pending)
            {
                return null;
            }

            record.Status = BorrowingRecordStatus.Approved;
            record.ApprovalDate = DateTime.UtcNow;
            record.ApproverUserId = adminId;

            await _context.SaveChangesAsync();

            if (record.RequesterUser != null)
            {
                await _notificationService.CreateNotificationAsync(record.RequesterUser.Id, $"Your document request (Document ID: {record.DocumentId}) has been approved. You can collect it from the archive.", $"/borrowing/{record.Id}");
            }

            var loaded = await _context.BorrowingRecords.Where(r => r.Id == record.Id).IncludeFullDetails().AsNoTracking().FirstAsync();
            return _mapper.Map<BorrowingRecordDto>(loaded);
        }

        public async Task<BorrowingRecordDto?> RejectRequestAsync(int recordId, int adminId)
        {
            var record = await _context.BorrowingRecords
                .Include(r => r.RequesterUser)
                .FirstOrDefaultAsync(r => r.Id == recordId);
            if (record == null || record.Status != BorrowingRecordStatus.Pending)
            {
                return null;
            }

            record.Status = BorrowingRecordStatus.Rejected;
            record.ApprovalDate = DateTime.UtcNow;
            record.ApproverUserId = adminId;

            await _context.SaveChangesAsync();

            if (record.RequesterUser != null)
            {
                await _notificationService.CreateNotificationAsync(record.RequesterUser.Id, $"Your document request (Document ID: {record.DocumentId}) has been rejected.", $"/borrowing/{record.Id}");
            }

            var loaded = await _context.BorrowingRecords.Where(r => r.Id == record.Id).IncludeFullDetails().AsNoTracking().FirstAsync();
            return _mapper.Map<BorrowingRecordDto>(loaded);
        }

        public async Task<BorrowingRecordDto?> CheckoutDocumentAsync(int recordId, int adminId)
        {
            var record = await _context.BorrowingRecords
                .Include(r => r.Document)
                .Include(r => r.RequesterUser)
                .FirstOrDefaultAsync(r => r.Id == recordId);
            if (record == null || record.Status != BorrowingRecordStatus.Approved)
            {
                return null;
            }

            record.Status = BorrowingRecordStatus.CheckedOut;
            record.CheckoutDate = DateTime.UtcNow;
            record.ApproverUserId = adminId;

            record.Document.Status = DocumentStatus.CheckedOut;

            await _context.SaveChangesAsync();

            if (record.RequesterUser != null && record.Document != null)
            {
                await _notificationService.CreateNotificationAsync(record.RequesterUser.Id, $"The document '{record.Document.Title}' has been collected from the archive.", $"/borrowing/{record.Id}");
            }

            var loaded = await _context.BorrowingRecords.Where(r => r.Id == record.Id).IncludeFullDetails().AsNoTracking().FirstAsync();
            return _mapper.Map<BorrowingRecordDto>(loaded);
        }

        public async Task<BorrowingRecordDto?> ReturnDocumentAsync(int recordId, int adminId)
        {
            var record = await _context.BorrowingRecords
                .Include(r => r.Document)
                .Include(r => r.RequesterUser)
                .FirstOrDefaultAsync(r => r.Id == recordId);
            if (record == null || record.Status != BorrowingRecordStatus.CheckedOut)
            {
                return null;
            }

            record.Status = BorrowingRecordStatus.Returned;
            record.ReturnDate = DateTime.UtcNow;

            record.Document.Status = DocumentStatus.Available;

            await _context.SaveChangesAsync();

            if (record.RequesterUser != null && record.Document != null)
            {
                await _notificationService.CreateNotificationAsync(record.RequesterUser.Id, $"You have successfully returned the document '{record.Document.Title}'.", $"/borrowing/{record.Id}");
            }

            var loaded = await _context.BorrowingRecords.Where(r => r.Id == record.Id).IncludeFullDetails().AsNoTracking().FirstAsync();
            return _mapper.Map<BorrowingRecordDto>(loaded);
        }

        public async Task<PagedList<BorrowingRecordDto>> GetAllPendingRequestsAsync(PagingParameters pagingParameters)
        {
            var query = _context.BorrowingRecords
                .Where(r => r.Status == BorrowingRecordStatus.Pending)
                .IncludeFullDetails()
                .OrderBy(r => r.RequestDate)
                .AsNoTracking();

            var dtoQuery = query.ProjectTo<BorrowingRecordDto>(_mapper.ConfigurationProvider);

            return await PagedList<BorrowingRecordDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<PagedList<BorrowingRecordDto>> GetAllApprovedRequestsAsync(PagingParameters pagingParameters)
        {
            var query = _context.BorrowingRecords
                .Where(r => r.Status == BorrowingRecordStatus.Approved)
                .IncludeFullDetails()
                .OrderBy(r => r.ApprovalDate)
                .AsNoTracking();

            var dtoQuery = query.ProjectTo<BorrowingRecordDto>(_mapper.ConfigurationProvider);

            return await PagedList<BorrowingRecordDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<PagedList<BorrowingRecordDto>> GetAllBorrowedDocumentsAsync(PagingParameters pagingParameters)
        {
            var query = _context.BorrowingRecords
                .Where(r => r.Status == BorrowingRecordStatus.CheckedOut || r.Status == BorrowingRecordStatus.Overdue)
                .IncludeFullDetails()
                .OrderBy(r => r.DueDate)
                .AsNoTracking();

            var dtoQuery = query.ProjectTo<BorrowingRecordDto>(_mapper.ConfigurationProvider);

            return await PagedList<BorrowingRecordDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<PagedList<BorrowingRecordDto>> GetAllBorrowingRecordsAsync(PagingParameters pagingParameters)
        {
            var query = _context.BorrowingRecords
                .AsNoTracking();

            // Apply filters
            if (pagingParameters.RequesterUserId.HasValue && pagingParameters.RequesterUserId.Value > 0)
            {
                query = query.Where(r => r.RequesterUserId == pagingParameters.RequesterUserId.Value);
            }

            if (pagingParameters.DocumentTypeIdForBorrowing.HasValue && pagingParameters.DocumentTypeIdForBorrowing.Value > 0)
            {
                query = query.Where(r => r.Document != null && r.Document.DocumentTypeId == pagingParameters.DocumentTypeIdForBorrowing.Value);
            }

            if (!string.IsNullOrWhiteSpace(pagingParameters.BorrowingStatus))
            {
                if (Enum.TryParse<BorrowingRecordStatus>(pagingParameters.BorrowingStatus, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            if (pagingParameters.DateFrom.HasValue)
            {
                query = query.Where(r => r.RequestDate >= pagingParameters.DateFrom.Value);
            }

            if (pagingParameters.DateTo.HasValue)
            {
                query = query.Where(r => r.RequestDate <= pagingParameters.DateTo.Value);
            }

            // Include navigation properties
            query = query
                .Include(r => r.Document)
                    .ThenInclude(d => d.DocumentType)
                .Include(r => r.RequesterUser)
                .Include(r => r.ApproverUser);

            // Order by request date descending (newest first)
            query = query.OrderByDescending(r => r.RequestDate);

            var dtoQuery = query.ProjectTo<BorrowingRecordDto>(_mapper.ConfigurationProvider);

            return await PagedList<BorrowingRecordDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<BorrowingRecordDto?> GetRecordByIdAsync(int id)
        {
            var record = await _context.BorrowingRecords
                .Where(r => r.Id == id)
                .IncludeFullDetails()
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return record == null ? null : _mapper.Map<BorrowingRecordDto>(record);
        }
    }

    public static class BorrowingRecordQueryExtensions
    {
        public static IQueryable<BorrowingRecord> IncludeFullDetails(this IQueryable<BorrowingRecord> query)
        {
            return query
                .Include(r => r.Document)
                .Include(r => r.RequesterUser)
                .Include(r => r.ApproverUser);
        }
    }
}
