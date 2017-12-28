using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Repositories.Mapping;
using ACTransit.CusRel.Repositories.Search;
using ACTransit.DataAccess.CustomerRelations;
using ACTransit.Framework.Extensions;

namespace ACTransit.CusRel.Repositories
{
    public class ReportRepository: IDisposable
    {
        private readonly TicketRepository ticketRepository;
        private readonly RequestState requestState;
        private CusRelEntities cusRelContext;

        #region Constructors / Initialization

        public ReportRepository(RequestState requestState = null)
        {
            ticketRepository = new TicketRepository();
            this.requestState = requestState;
            init();
        }

        public ReportRepository(TicketRepository ticketRepository, RequestState requestState)
        {
            this.ticketRepository = ticketRepository;
            this.requestState = requestState;
            init();
        }

        private void init()
        {
        }

        private void InitCusRelContext()
        {
            if (cusRelContext == null)
                cusRelContext = new CusRelEntities();
            init();
        }

        #endregion

        // =============================================================

        #region Dispose

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (cusRelContext != null)
                    cusRelContext.Dispose();
                ticketRepository.Dispose();
            }
                
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
       
        // =============================================================

        private string CleanGroupContact(string GroupContact)
        {
            return GroupContact != null ? Regex.Replace(GroupContact, @".{1,2}\.\s+", "") : null;
        }

        private string CleanReasons(string Reasons)
        {
            return Reasons != null ? Regex.Replace(Reasons, @"[^;]{1,2}\.\s+", "") : null;
        }

        #region Public Methods

        public AssignedToReportResult AssignedToReport(AssignedToReportParams Params)
        {
            var searchCriteria = new TicketSearchParams
            {
                Assignment = new Assignment
                {
                    Employee = Params.AssignedTo
                },
                ExcludeTicketStatusList = new List<string>
                {
                    TicketStatus.Closed.ToString().PascalCaseToDescription(),
                    TicketStatus.ClosedDuplicate.ToString().PascalCaseToDescription(),
                    TicketStatus.ClosedTooLate.ToString().PascalCaseToDescription(),
                },
                IsForwardOrder = true
            };
            var items = searchCriteria.Search(requestState).ToAssignedToReport();
            foreach (var item in items)
                item.Reasons = CleanReasons(item.Reasons);
            return new AssignedToReportResult
            {
                OpenTicketCount = items.Count(),
                Report = new AssignedToReport
                {
                    Items = items
                }
            };
        }

        public ForActionReportResult ForActionReport(ForActionReportParams Params)
        {
            var searchCriteria = new TicketSearchParams
            {
                Assignment = new Assignment
                {
                    GroupContact = new GroupContact(Params.GroupContact)
                },
                ExcludeTicketStatusList = new List<string>
                {
                    TicketStatus.Closed.ToString().PascalCaseToDescription(),
                    TicketStatus.ClosedDuplicate.ToString().PascalCaseToDescription(),
                    TicketStatus.ClosedTooLate.ToString().PascalCaseToDescription(),
                },
                IsForwardOrder = true
            };
            var items = searchCriteria.Search(requestState).ToForActionReport();
            foreach (var item in items)
            {
                item.Reasons = CleanReasons(item.Reasons);
                item.GroupContact = CleanGroupContact(item.GroupContact);
            }
            return new ForActionReportResult
            {
                OpenTicketCount = items.Count(),
                Report = new ForActionReport
                {
                    Items = items
                }
            };
        }

        public ReadyToCloseReportResult ReadyToCloseReport(ReportParams Params)
        {
            var searchCriteria = new TicketSearchParams
            {
                IncludeTicketStatusList = new List<string>
                {
                    TicketStatus.ReadyToClose.ToString().PascalCaseToDescription(),
                    TicketStatus.ReadyToCloseDuplicate.ToString().PascalCaseToDescription(),
                    TicketStatus.ReadyToCloseTooLate.ToString().PascalCaseToDescription(),
                },
                IncludeContactHistory = true
            };
            var items = searchCriteria.Search(requestState).ToReadyToCloseReport();
            foreach (var item in items)
            {
                item.Reasons = CleanReasons(item.Reasons);
                item.GroupContact = CleanGroupContact(item.GroupContact);
            }

            return new ReadyToCloseReportResult
            {
                Report = new ReadyToCloseReport
                {
                    Items = items
                }
            };
        }

        public LostFoundReportResult LostFoundReport(LostFoundReportParams Params)
        {
            var codes = ticketRepository.GetComplaintCodes();
            var lostAndFoundCode = codes.ComplaintCodes.First(c => c.Description.ToLower().Contains("lost property"));
            var searchCriteria = new TicketSearchParams
            {
                LostItem = new LostItem
                {
                    Category = Params.LostItemCategory,
                    Type = Params.LostItemType
                },
                IncidentAtFrom = Params.RangeStart,
                IncidentAtTo = Params.RangeEnd,
                ReasonCode1 = lostAndFoundCode.ToString(),
                ExcludeTicketStatusList = new List<string>
                {
                    TicketStatus.Closed.ToString().PascalCaseToDescription(),
                    TicketStatus.ClosedDuplicate.ToString().PascalCaseToDescription(),
                    TicketStatus.ClosedTooLate.ToString().PascalCaseToDescription(),
                }
            };
            var items = searchCriteria.Search(requestState).ToLostFoundReport();
            return new LostFoundReportResult
            {
                Report = new LostFoundReport
                {
                    Items = items
                }
            };
        }

        public RejectedReportResult RejectedReport(ReportParams Params)
        {
            var searchCriteria = new TicketSearchParams
            {
                Status = TicketStatus.Rejected,
                ReceivedAtFrom = Params.RangeStart,
                ReceivedAtTo = Params.RangeEnd,
                IncludeResearchHistory = true,
            };
            var items = searchCriteria.Search(requestState).ToRejectedReport();
            foreach (var item in items)
            {
                item.Reasons = CleanReasons(item.Reasons);
                item.GroupContact = CleanGroupContact(item.GroupContact);
            }

            return new RejectedReportResult
            {
                Report = new RejectedReport
                {
                    Items = items
                }
            };
        }

        private const string OpenTicketsQuery =
@"CREATE TABLE #tblOpenTickets(
    [RefCode] [char](2) NULL,
    [ReceivedDateTime] [datetime] NULL,
    [DaysOpen] [int] NULL,
    [IsVisible] [bit] NULL,
) 

INSERT INTO #tblOpenTickets
    SELECT ISNULL(NULLIF(REPLACE(LEFT(a.ForAction, 2), '.', ''), ''), '0'), 
           a.ReceivedDateTime, 
           DATEDIFF(day, ReceivedDateTime, GETDATE()) as DaysOpen,
		 b.IsVisible
    FROM   dbo.tblContacts a 
    LEFT JOIN dbo.tblCustomerComplaintCodes b 
           ON b.ComplaintCode = LEFT(a.Reasons, 2) 
    WHERE  ResolvedDateTime IS NULL 
           AND ReceivedDateTime IS NOT NULL 
           AND (@p0 is null or ReceivedDateTime >= @p0)
           AND (@p1 is null or ReceivedDateTime <= @p1) 
           AND a.ForAction is not null
		 AND LEN(RTRIM(a.ForAction) + 'x') - 1 > 0

CREATE TABLE #result(
    [Code] varchar(5) NULL,
    [Description] char(30) NULL,
    [NewCount] int NOT NULL default(0),
    [WorkingCount] int NOT NULL default(0),
    [Over30Count] int NOT NULL default(0),
    [TotalCount] int NOT NULL default(0)
)

INSERT INTO #result
    SELECT REFER_CODE, 
        REFER_DESC, 
        (SELECT Count (1) 
            FROM   #tblOpenTickets 
            WHERE  RefCode = a.REFER_CODE
                AND DaysOpen >= 0 AND DaysOpen < 11), 
        (SELECT Count (1) 
            FROM   #tblOpenTickets 
            WHERE  RefCode = a.REFER_CODE
                AND DaysOpen >= 11 AND DaysOpen < 30), 
        (SELECT Count (1) 
            FROM   #tblOpenTickets 
            WHERE  RefCode = a.REFER_CODE
                AND DaysOpen >= 30), 
        (SELECT Count (1) 
            FROM   #tblOpenTickets 
            WHERE  RefCode = a.REFER_CODE)
    FROM dbo.tblCustomerReferenceCodes a 
    WHERE a.IsVisible = 1
    GROUP BY REFER_CODE, REFER_DESC
    ORDER BY REFER_DESC 

INSERT INTO #result
    SELECT NULL, 'Totals', SUM(NewCount), SUM(WorkingCount), SUM(Over30Count), SUM(TotalCount)
    FROM #result;

SELECT * FROM #result

DROP TABLE #result;
DROP TABLE #tblOpenTickets;";

        private List<OpenTicketsReportTableItem> GetOpenTickets(DateTime? StartDate, DateTime? EndDate)
        {
            InitCusRelContext();
            return cusRelContext.Database.SqlQuery<OpenTicketsReportTableItem>(OpenTicketsQuery, StartDate, EndDate).ToList();
        }

        private const string OpenTicketsByGroupQuery =
@"CREATE TABLE #tblOpenTickets(
    [ComplaintGroup] varchar(12) NULL,
    [ComplaintCode] varchar(5) NULL,
    [ReceivedDateTime] [datetime] NULL,
    [DaysOpen] [int] NULL,
    [IsVisible] [bit] NULL,
    [IsAda] [bit] NULL,
) 

INSERT INTO #tblOpenTickets
    SELECT b.ComplaintGroup,
		 b.ComplaintCode,
           a.ReceivedDateTime, 
           DATEDIFF(day, ReceivedDateTime, GETDATE()) as DaysOpen,
		 b.IsVisible,
		 CASE WHEN a.ADAComplaint = 'Y' THEN 1 ELSE 0 END
    FROM   dbo.tblContacts a 
    LEFT JOIN dbo.tblCustomerComplaintCodes b 
           ON b.ComplaintCode = LEFT(a.Reasons, 2) 
    WHERE  ResolvedDateTime IS NULL 
		 AND b.IsVisible = 1
           AND ReceivedDateTime IS NOT NULL 
           AND (@p0 is null or ReceivedDateTime >= @p0)
           AND (@p1 is null or ReceivedDateTime <= @p1) 
           AND (b.ComplaintGroup is not null OR a.ADAComplaint = 'Y')

CREATE TABLE #result(
    [Code] varchar(5) NULL,
    [Description] char(30) NULL,
    [NewCount] int NOT NULL default(0),
    [WorkingCount] int NOT NULL default(0),
    [Over30Count] int NOT NULL default(0),
    [TotalCount] int NOT NULL default(0)
)

INSERT INTO #result
    SELECT '',
	   'ADA',
	   SUM(CASE WHEN ot.DaysOpen <= 3 THEN 1 ELSE 0 END),
	   SUM(CASE WHEN ot.DaysOpen > 3 AND ot.DaysOpen <= 30 THEN 1 ELSE 0 END),
	   SUM(CASE WHEN ot.DaysOpen > 30 THEN 1 ELSE 0 END),
	   SUM(1)
    FROM #tblOpenTickets ot
    WHERE ot.IsAda = 1 OR ot.ComplaintCode in (SELECT ComplaintCode FROM tblCustomerComplaintCodes WHERE ComplaintGroup = 'ADA')

    UNION

    SELECT '',
        a.ComplaintGroup, 
        ISNULL((SELECT SUM(1) 
         FROM #tblOpenTickets 
         WHERE ComplaintGroup = a.ComplaintGroup 
		  AND IsAda = 0
            AND DaysOpen <= 3),0), 
        ISNULL((SELECT SUM(1) 
         FROM #tblOpenTickets 
         WHERE ComplaintGroup = a.ComplaintGroup 
		  AND IsAda = 0
                AND DaysOpen > 3 AND DaysOpen <= 30),0), 
        ISNULL((SELECT SUM(1) 
            FROM #tblOpenTickets 
            WHERE ComplaintGroup = a.ComplaintGroup 
			 AND IsAda = 0
                AND DaysOpen > 30),0), 
        ISNULL((SELECT SUM(1) 
            FROM #tblOpenTickets 
            WHERE ComplaintGroup = a.ComplaintGroup
			 AND IsAda = 0),0)
    FROM dbo.tblCustomerComplaintCodes a 
    WHERE a.ComplaintGroup != 'ADA'
	   AND a.ComplaintGroup is not null
    GROUP BY a.ComplaintGroup

INSERT INTO #result
    SELECT NULL, 'Totals', SUM(NewCount), SUM(WorkingCount), SUM(Over30Count), SUM(TotalCount)
    FROM #result;

SELECT * FROM #result

DROP TABLE #result;
DROP TABLE #tblOpenTickets;";

        private List<OpenTicketsReportTableItem> GetOpenTicketsByGroup(DateTime? StartDate, DateTime? EndDate)
        {
            InitCusRelContext();
            return cusRelContext.Database.SqlQuery<OpenTicketsReportTableItem>(OpenTicketsByGroupQuery, StartDate, EndDate).ToList();
        }

        public OpenTicketsReportResult OpenTicketsReport(ReportParams Params)
        {
            return new OpenTicketsReportResult
            {
                Report = new OpenTicketsReport
                {
                    Items = GetOpenTickets(Params.RangeStart, Params.RangeEnd),
                    GroupItems = GetOpenTicketsByGroup(Params.RangeStart, Params.RangeEnd)
                }
            };
        }

        private const string OpenTicketsStatusQuery =
 @"SELECT 
    SUBSTRING(ForAction, 1, CHARINDEX('.', ForAction) - 1) as Code,
    SUBSTRING(ForAction, CHARINDEX('.', ForAction) + 2, LEN(ForAction)) as [Description],
    NewCount = SUM(case TicketStatus when 'New' then 1 else 0 end),
    AssignedCount = SUM(case TicketStatus when 'Assigned' then 1 else 0 end),
    PendingContactCount = SUM(case TicketStatus when 'Pending Contact' then 1 else 0 end),
    RejectedCount = SUM(case TicketStatus when 'Rejected' then 1 else 0 end),
    ReadyToCloseCount = SUM(case TicketStatus when 'Ready To Close' then 1 else 0 end)
INTO #table
FROM dbo.tblContacts
WHERE ResolvedDateTime IS NULL
    AND (ForAction != '')
    AND (@p0 is null or ReceivedDateTime >= @p0)
    AND (@p1 is null or ReceivedDateTime <= @p1)
    AND (@p2 is null or ADAComplaint = @p2)
GROUP BY ForAction

INSERT INTO #table
    SELECT 
        NULL,
        'Totals', 
        SUM(NewCount), 
        SUM(AssignedCount), 
        SUM(PendingContactCount), 
        SUM(RejectedCount),
        SUM(ReadyToCloseCount)
    FROM #table

SELECT * FROM #table ORDER BY CASE WHEN Code IS NULL THEN 'zzz' ELSE [Description] END";

        private List<OpenTicketsStatusReportTableItem> GetOpenTicketsStatus(DateTime? StartDate, DateTime? EndDate, bool? IsAda)
        {
            InitCusRelContext();
            var adaValue = IsAda.HasValue ? (IsAda.Value ? "Y" : "N") : null;
            return cusRelContext.Database.SqlQuery<OpenTicketsStatusReportTableItem>(OpenTicketsStatusQuery, StartDate, EndDate, adaValue).ToList();
        }

        public OpenTicketsStatusReportResult OpenTicketsStatusReport(OpenTicketStatusReportParams Params)
        {
            return new OpenTicketsStatusReportResult
            {
                Report = new OpenTicketsStatusReport
                {
                    Items = GetOpenTicketsStatus(Params.RangeStart, Params.RangeEnd, Params.IsAda)
                }
            };
        }

        private const string ProductivityQuery =

@"SELECT REFER_CODE as Code, 
       REFER_DESC as [Description], 
       (SELECT Count(1) 
        FROM   tblContacts 
        WHERE  LEN(forAction) > 1 AND LEFT(forAction, Charindex('.', forAction) - 1) = REFER_CODE 
               AND ReceivedDateTime >= @p0 
               AND ReceivedDateTime <= @p1) AS ReceivedCount, 
       (SELECT Count(1) 
        FROM   tblContacts 
        WHERE  LEN(forAction) > 1 AND LEFT(forAction, Charindex('.', forAction) - 1) = REFER_CODE 
               AND ResolvedDateTime IS NOT NULL 
               AND ResolvedDateTime >= @p0 
               AND ResolvedDateTime <= @p1) AS ClosedCount 
INTO #table
FROM  dbo.tblCustomerReferenceCodes
WHERE IsVisible = 1
ORDER BY REFER_DESC 

INSERT INTO #table
    SELECT 
        '',
        'Totals', 
        SUM([ReceivedCount]), 
        SUM([ClosedCount])
    FROM #table

SELECT * FROM #table ORDER BY CASE WHEN Code = '' THEN 'zzz' ELSE [Description] END";

        private List<ProductivityReportTableItem> GetProductivity(DateTime? StartDate, DateTime? EndDate)
        {
            InitCusRelContext();
            return cusRelContext.Database.SqlQuery<ProductivityReportTableItem>(ProductivityQuery, StartDate, EndDate).ToList();
        }

        public ProductivityReportResult ProductivityReport(ReportParams Params)
        {
            return new ProductivityReportResult
            {
                Report = new ProductivityReport
                {
                    Items = GetProductivity(Params.RangeStart, Params.RangeEnd),
                    GroupItems = GetProductivity(Params.RangeStart, Params.RangeEnd)
                }
            };
        }


        #endregion

        // =============================================================
    }
}
