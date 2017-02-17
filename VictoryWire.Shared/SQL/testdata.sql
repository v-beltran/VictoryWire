--ACCOUNTS
SET IDENTITY_INSERT [dbo].[Account] ON
INSERT [dbo].[Account] ([account_id], [created], [last_login], [last_modified], [first_name], [last_name], [email], [password], [type]) 
VALUES (100, '2017-01-01 0:00:00', N'0001-01-01 0:00:00', N'0001-01-01 0:00:00', 'James', 'Green', 'jgreen@activision.com', '114663AB194EDCB3F61D409883CE4AE6C3C2F9854194095A5385011D15BECBEF', 2);
GO

INSERT [dbo].[Account] ([account_id], [created], [last_login], [last_modified], [first_name], [last_name], [email], [password], [type]) 
VALUES (101, '2017-02-10 0:00:00', N'0001-01-01 0:00:00', N'0001-01-01 0:00:00', 'Willis', 'Fair', 'wfair@apple.com', '114663AB194EDCB3F61D409883CE4AE6C3C2F9854194095A5385011D15BECBEF', 0);
GO

INSERT [dbo].[Account] ([account_id], [created], [last_login], [last_modified], [first_name], [last_name], [email], [password], [type]) 
VALUES (102, '2017-01-10 0:00:00', N'0001-01-01 0:00:00', N'0001-01-01 0:00:00', 'George', 'Anderson', 'ganderson@intel.com', '114663AB194EDCB3F61D409883CE4AE6C3C2F9854194095A5385011D15BECBEF', 1);
GO

INSERT [dbo].[Account] ([account_id], [created], [last_login], [last_modified], [first_name], [last_name], [email], [password], [type]) 
VALUES (103, '2017-01-15 0:00:00', N'0001-01-01 0:00:00', N'0001-01-01 0:00:00', 'Bill', 'Stokes', 'bstokes@jpmorgan.com', '114663AB194EDCB3F61D409883CE4AE6C3C2F9854194095A5385011D15BECBEF', 2);
GO

INSERT [dbo].[Account] ([account_id], [created], [last_login], [last_modified], [first_name], [last_name], [email], [password], [type]) 
VALUES (104, '2017-02-01 0:00:00', N'0001-01-01 0:00:00', N'0001-01-01 0:00:00', 'Sharon', 'Jones', 'sjones@boa.com', '114663AB194EDCB3F61D409883CE4AE6C3C2F9854194095A5385011D15BECBEF', 0);
GO
SET IDENTITY_INSERT [dbo].[Account] OFF



--CONTACTS
SET IDENTITY_INSERT [dbo].[Contact] ON
INSERT [dbo].[Contact] ([contact_id], [last_modified], [address], [city], [state], [zip], [email], [phone]) 
VALUES (1000, '2017-01-01 0:00:00', '3100 Ocean Park Boulevard', 'Santa Monica', 'California', '90405', 'jgreen@activision.com', '310-255-200');
GO

INSERT [dbo].[Contact] ([contact_id], [last_modified], [address], [city], [state], [zip], [email], [phone]) 
VALUES (1001, '2017-01-01 0:00:00', '1 Infinite Loop', 'Cupertino', 'California', '95014', 'wfair@apple.com', '800-692-7753');
GO

INSERT [dbo].[Contact] ([contact_id], [last_modified], [address], [city], [state], [zip], [email], [phone]) 
VALUES (1002, '2017-01-01 0:00:00', '2200 Mission College Blvd.', 'Santa Clara', 'California', '95054', 'ganderson@intel.com', '408-765-8080');
GO

INSERT [dbo].[Contact] ([contact_id], [last_modified], [address], [city], [state], [zip], [email], [phone]) 
VALUES (1003, '2017-01-01 0:00:00', '270 Park Avenue', 'Manhatten', 'New York', '10017', 'bstokes@jpmorgan.com', '310-255-200');
GO

INSERT [dbo].[Contact] ([contact_id], [last_modified], [address], [city], [state], [zip], [email], [phone]) 
VALUES (1004, '2017-01-01 0:00:00', '100 North Tryon St.', 'Charlotte', 'North Carolina', '28255', 'sjones@boa.com', '800-432-1000');
GO
SET IDENTITY_INSERT [dbo].[Contact] OFF




--COMPANIES
INSERT [dbo].[Company] ([account_id], [contact_id], [last_modified], [name], [summary], [website], [industry], [logo]) 
VALUES (100, 1000, '2017-01-01 0:00:00', 'Activision Blizzard, Inc.', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque commodo rutrum diam quis fermentum. Aenean rhoncus ante in venenatis laoreet.', 'http://www.activisionblizzard.com', 'Technology', '');
GO

INSERT [dbo].[Company] ([account_id], [contact_id], [last_modified], [name], [summary], [website], [industry], [logo]) 
VALUES (101, 1001, '2017-01-01 0:00:00', 'Apple', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque commodo rutrum diam quis fermentum. Aenean rhoncus ante in venenatis laoreet.', 'http://www.apple.com', 'Technology', '');
GO

INSERT [dbo].[Company] ([account_id], [contact_id], [last_modified], [name], [summary], [website], [industry], [logo]) 
VALUES (102, 1002, '2017-01-01 0:00:00', 'Intel', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque commodo rutrum diam quis fermentum. Aenean rhoncus ante in venenatis laoreet.', 'http://www.intel.com', 'Technology', '');
GO

INSERT [dbo].[Company] ([account_id], [contact_id], [last_modified], [name], [summary], [website], [industry], [logo]) 
VALUES (103, 1003, '2017-01-01 0:00:00', 'J P Morgan Chase & Co.', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque commodo rutrum diam quis fermentum. Aenean rhoncus ante in venenatis laoreet.', 'http://www.jpmorgan.com', 'Finance', '');
GO

INSERT [dbo].[Company] ([account_id], [contact_id], [last_modified], [name], [summary], [website], [industry], [logo]) 
VALUES (104, 1004, '2017-01-01 0:00:00', 'Bank of America Corporation', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque commodo rutrum diam quis fermentum. Aenean rhoncus ante in venenatis laoreet.', 'https://www.bankofamerica.com', 'Finance', '');
GO



--EMPLOYEES
SET IDENTITY_INSERT [dbo].[Employee] ON
INSERT [dbo].[Employee] ([employee_id], [account_id], [contact_id], [last_modified], [hired], [terminated], [first_name], [last_name], [title], [department], [pay_rate_type], [pay_rate]) 
VALUES (2000, 100, 1000, '0001-01-01 0:00:00', '2016-09-01 0:00:00', '0001-01-01 0:00:00', 'Rachel', 'Kirsch', 'Developer', 'Development', '1', 75000.00);
GO

INSERT [dbo].[Employee] ([employee_id], [account_id], [contact_id], [last_modified], [hired], [terminated], [first_name], [last_name], [title], [department], [pay_rate_type], [pay_rate]) 
VALUES (2001, 101, 1001, '0001-01-01 0:00:00', '2016-10-01 0:00:00', '0001-01-01 0:00:00', 'Frederick', 'Rogers', 'Developer', 'Development', '0', 33.75);
GO

INSERT [dbo].[Employee] ([employee_id], [account_id], [contact_id], [last_modified], [hired], [terminated], [first_name], [last_name], [title], [department], [pay_rate_type], [pay_rate]) 
VALUES (2002, 102, 1002, '0001-01-01 0:00:00', '2016-11-01 0:00:00', '0001-01-01 0:00:00', 'Ophelia', 'Dias', 'Developer', 'Development', '0', 40.00);
GO

INSERT [dbo].[Employee] ([employee_id], [account_id], [contact_id], [last_modified], [hired], [terminated], [first_name], [last_name], [title], [department], [pay_rate_type], [pay_rate]) 
VALUES (2003, 103, 1003, '0001-01-01 0:00:00', '2016-12-01 0:00:00', '0001-01-01 0:00:00', 'Jody', 'Maxwell', 'Developer', 'Development', '0', 20.00);
GO

INSERT [dbo].[Employee] ([employee_id], [account_id], [contact_id], [last_modified], [hired], [terminated], [first_name], [last_name], [title], [department], [pay_rate_type], [pay_rate]) 
VALUES (2004, 104, 1004, '0001-01-01 0:00:00', '2017-01-01 0:00:00', '0001-01-01 0:00:00', 'Kenneth', 'Anderson', 'Developer', 'Development', '1', 150000.00);
GO

INSERT [dbo].[Employee] ([employee_id], [account_id], [contact_id], [last_modified], [hired], [terminated], [first_name], [last_name], [title], [department], [pay_rate_type], [pay_rate]) 
VALUES (2005, 104, 1004, '0001-01-01 0:00:00', '2017-01-01 0:00:00', '2017-02-01 0:00:00', 'Jacob', 'Gray', 'Developer', 'Development', '1', 25.00);
GO
SET IDENTITY_INSERT [dbo].[Employee] OFF




--PAYROLLS
SET IDENTITY_INSERT [dbo].[Payroll] ON
INSERT [dbo].[Payroll] ([payroll_id], [account_id], [last_modified], [week_ending]) 
VALUES (3000, 100, '0001-01-01 0:00:00', '2017-02-12 0:00:00');
GO

INSERT [dbo].[Payroll] ([payroll_id], [account_id], [last_modified], [week_ending]) 
VALUES (3001, 101, '0001-01-01 0:00:00', '2017-02-12 0:00:00');
GO

INSERT [dbo].[Payroll] ([payroll_id], [account_id], [last_modified], [week_ending]) 
VALUES (3002, 102, '0001-01-01 0:00:00', '2017-02-12 0:00:00');
GO

INSERT [dbo].[Payroll] ([payroll_id], [account_id], [last_modified], [week_ending]) 
VALUES (3003, 103, '0001-01-01 0:00:00', '2017-02-12 0:00:00');
GO

INSERT [dbo].[Payroll] ([payroll_id], [account_id], [last_modified], [week_ending]) 
VALUES (3004, 104, '0001-01-01 0:00:00', '2017-02-12 0:00:00');
GO
SET IDENTITY_INSERT [dbo].[Payroll] OFF



--PAYROLL DETAILS
SET IDENTITY_INSERT [dbo].[Payroll_Details] ON
INSERT [dbo].[Payroll_Details] ([payroll_details_id], [payroll_id], [employee_id], [last_modified], [hours_std_worked], [hours_ot_worked], [deductions]) 
VALUES (4000, 3000, 2000, '0001-01-01 0:00:00', 40, 10, 100.00);
GO

INSERT [dbo].[Payroll_Details] ([payroll_details_id], [payroll_id], [employee_id], [last_modified], [hours_std_worked], [hours_ot_worked], [deductions]) 
VALUES (4001, 3001, 2001, '0001-01-01 0:00:00', 40, 5, 200.00);
GO

INSERT [dbo].[Payroll_Details] ([payroll_details_id], [payroll_id], [employee_id], [last_modified], [hours_std_worked], [hours_ot_worked], [deductions]) 
VALUES (4002, 3002, 2002, '0001-01-01 0:00:00', 40, 0, 300.00);
GO

INSERT [dbo].[Payroll_Details] ([payroll_details_id], [payroll_id], [employee_id], [last_modified], [hours_std_worked], [hours_ot_worked], [deductions]) 
VALUES (4003, 3003, 2003, '0001-01-01 0:00:00', 40, 0, 400.00);
GO

INSERT [dbo].[Payroll_Details] ([payroll_details_id], [payroll_id], [employee_id], [last_modified], [hours_std_worked], [hours_ot_worked], [deductions]) 
VALUES (4004, 3004, 2004, '0001-01-01 0:00:00', 40, 0, 500.00);
GO
SET IDENTITY_INSERT [dbo].[Payroll_Details] OFF