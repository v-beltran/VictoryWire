CREATE TABLE [dbo].[Account](
	[account_id] [int] IDENTITY(1,1) NOT NULL,
	[created] [datetime2] NOT NULL,
	[last_login] [datetime2] NOT NULL,  
	[last_modified] [datetime2] NOT NULL,
	[first_name] [nvarchar](100) NOT NULL,
	[last_name] [nvarchar](100) NULL,
	[email] [nvarchar](255) NOT NULL,
	[password] [nvarchar](64) NOT NULL,
	[type] [int] NOT NULL
)
GO
ALTER TABLE [dbo].[Account] ADD CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([account_id] ASC)
GO

CREATE TABLE [dbo].[Contact](
	[contact_id] [int] IDENTITY(1,1) NOT NULL,
	[last_modified] [datetime2] NOT NULL,
	[address] [nvarchar](100) NOT NULL,
	[city] [nvarchar](100) NULL,
	[state] [nvarchar](100) NOT NULL,
	[zip] [nvarchar](10) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[phone] [nvarchar](12) NOT NULL,
)
GO
ALTER TABLE [dbo].[Contact] ADD CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([contact_id] ASC)
GO

CREATE TABLE [dbo].[Company](
	[account_id] [int] NOT NULL,
	[contact_id] [int] NOT NULL,
	[last_modified] [datetime2] NOT NULL,
	[name] [nvarchar](100) NOT NULL,  
	[summary] [nvarchar](255) NOT NULL,
	[website] [nvarchar](100) NOT NULL,
	[industry] [nvarchar](100) NULL,
	[logo] [nvarchar](255) NOT NULL
)
GO
ALTER TABLE [dbo].[Company] ADD CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([account_id] ASC)
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD CONSTRAINT [FK_dbo.Company_dbo.Account_account_id] FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD CONSTRAINT [FK_dbo.Company_dbo.Contact_contact_id] FOREIGN KEY([contact_id])
REFERENCES [dbo].[Contact] ([contact_id])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_dbo.Company_dbo.Account_account_id]
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_dbo.Company_dbo.Contact_contact_id]
GO

CREATE TABLE [dbo].[Employee](
	[employee_id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [int] NOT NULL,
	[contact_id] [int] NOT NULL,
	[last_modified] [datetime2] NOT NULL,
	[hired] [datetime2] NOT NULL,
	[terminated] [datetime2] NOT NULL, 
	[first_name] [nvarchar](100) NOT NULL,
	[last_name] [nvarchar](100) NOT NULL,
	[title] [nvarchar](100) NULL,
	[department] [nvarchar](100) NOT NULL,
	[pay_rate_type] int NOT NULL,
	[pay_rate] [decimal](12,2) NOT NULL
)
GO
ALTER TABLE [dbo].[Employee] ADD CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([employee_id] ASC)
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD CONSTRAINT [FK_dbo.Employee_dbo.Company_account_id] FOREIGN KEY([account_id])
REFERENCES [dbo].[Company] ([account_id])
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD CONSTRAINT [FK_dbo.Employee_dbo.Contact_contact_id] FOREIGN KEY([contact_id])
REFERENCES [dbo].[Contact] ([contact_id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_dbo.Employee_dbo.Company_account_id]
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_dbo.Employee_dbo.Contact_contact_id]
GO

CREATE TABLE [dbo].[Payroll](
	[payroll_id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [int] NOT NULL,
	[last_modified] [datetime2] NOT NULL,
	[week_ending] [datetime2] NOT NULL
)
GO
ALTER TABLE [dbo].[Payroll] ADD CONSTRAINT [PK_Payroll_Company] PRIMARY KEY CLUSTERED ([payroll_id] ASC)
GO
ALTER TABLE [dbo].[Payroll] WITH CHECK ADD CONSTRAINT [FK_dbo.Payroll_dbo.Company_account_id] FOREIGN KEY([account_id])
REFERENCES [dbo].[Company] ([account_id])
GO
ALTER TABLE [dbo].[Payroll] CHECK CONSTRAINT [FK_dbo.Payroll_dbo.Company_account_id]
GO

CREATE TABLE [dbo].[Payroll_Details](
	[payroll_details_id] [int] IDENTITY(1,1) NOT NULL,
	[payroll_id] [int] NOT NULL,
	[employee_id] [int] NOT NULL,
	[last_modified] [datetime2] NOT NULL,
	[hours_std_worked] [int] NOT NULL,
	[hours_ot_worked] [int] NOT NULL,
	[deductions] [decimal](12,2) NOT NULL
)
GO
ALTER TABLE [dbo].[Payroll_Details] ADD CONSTRAINT [PK_Payroll_Details] PRIMARY KEY CLUSTERED ([payroll_details_id] ASC)
GO
ALTER TABLE [dbo].[Payroll_Details] WITH CHECK ADD CONSTRAINT [FK_dbo.Payroll_Details_dbo.Payroll_payroll_id] FOREIGN KEY([payroll_id])
REFERENCES [dbo].[Payroll] ([payroll_id])
GO
ALTER TABLE [dbo].[Payroll_Details] WITH CHECK ADD CONSTRAINT [FK_dbo.Payroll_Details_dbo.Employee_employee_id] FOREIGN KEY([employee_id])
REFERENCES [dbo].[Employee] ([employee_id])
GO
ALTER TABLE [dbo].[Payroll_Details] CHECK CONSTRAINT [FK_dbo.Payroll_Details_dbo.Payroll_payroll_id]
GO
ALTER TABLE [dbo].[Payroll_Details] CHECK CONSTRAINT [FK_dbo.Payroll_Details_dbo.Employee_employee_id]
GO