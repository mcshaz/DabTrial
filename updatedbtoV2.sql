use dabtrial_com_participantdata
go

sp_RENAME 'Users.[ProfessionalRoleId]' , 'ProfessionalRole', 'COLUMN'
go

Alter table Users
alter column ProfessionalRole Int Not Null
go

sp_RENAME 'AdverseEvents.[AdverseEventId]' , 'Id', 'COLUMN'
go

Alter Table ParticipantDeaths
add ReportingUserId int not null
go

ALTER TABLE [dbo].[ParticipantDeaths]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ParticipantDeaths_dbo.Users_UserId] FOREIGN KEY([ReportingUserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE NO ACTION
GO

ALTER TABLE [dbo].[ParticipantDeaths] CHECK CONSTRAINT [FK_dbo.ParticipantDeaths_dbo.Users_UserId]
GO

Alter Table ParticipantDeaths
add ReportingTimeLocal DateTime not null
go

sp_RENAME 'ParticipantDeaths.[Time]' , 'EventTime', 'COLUMN'
go

Alter Table ParticipantWithdrawals
add ReportingUserId int null
go

ALTER TABLE [dbo].[ParticipantWithdrawals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ParticipantWithdrawals_dbo.Users_UserId] FOREIGN KEY([ReportingUserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE NO ACTION
GO

ALTER TABLE [dbo].[ParticipantWithdrawals] CHECK CONSTRAINT [FK_dbo.ParticipantWithdrawals_dbo.Users_UserId]
GO

Alter Table ParticipantWithdrawals
add ReportingTimeLocal DateTime null
go

update ParticipantWithdrawals
set ReportingUserId = w.UserId, ReportingTimeLocal = w.reportingTimeLocal
from
  (select dateadd(hh, case when (a.EventDateUTC < '2014-4-6 00:00:00') then 11
  else 10
  end, a.EventDateUTC) reportingTimeLocal, u.UserId, 
  cast(SUBSTRING(
    a.NewValue, 
    CHARINDEX('"', a.NewValue) + 1, 
    LEN(a.NewValue) - CHARINDEX('"', a.NewValue) - CHARINDEX('"', REVERSE(a.NewValue))
	) as int) participantId
  FROM [dabtrial_com_participantdata].[dbo].[AuditLogEntries] a
  inner join dbo.Users u on a.UserName = u.UserName
  where a.TableName='ParticipantWithdrawal' and a.EventType='C') w
inner join ParticipantWithdrawals on w.participantId = ParticipantWithdrawals.Id
go

Alter Table ParticipantWithdrawals
alter column ReportingUserId int not null
go

Alter Table ParticipantWithdrawals
alter column ReportingTimeLocal DateTime not null
go

sp_RENAME 'ParticipantWithdrawals.[Time]' , 'EventTime', 'COLUMN'
go

sp_RENAME 'ParticipantWithdrawals.[Reason]' , 'Details', 'COLUMN'
go

sp_RENAME 'ProtocolViolations.[ViolationId]' , 'Id', 'COLUMN'
go

sp_RENAME 'ProtocolViolations.[TimeOfViolation]' , 'EventTime', 'COLUMN'
go