USE NexusSupport;
GO

INSERT INTO Departments (Name) VALUES
('IT Support'),
('Network Operations'),
('Security');

INSERT INTO Employees (FirstName, LastName, Email, Role, DepartmentId, StartDate) VALUES
('Sarah',  'Mitchell',   'sarah.mitchell@nexussupport.com',  'Support Manager',           1, '2019-03-15'),
('James',  'Crawford',   'james.crawford@nexussupport.com',  'Senior Support Engineer',   1, '2020-06-01'),
('Priya',  'Patel',      'priya.patel@nexussupport.com',     'Support Engineer',          1, '2021-09-12'),
('Tom',    'Hargreaves', 'tom.hargreaves@nexussupport.com',  'Support Engineer',          1, '2022-01-10'),
('Lucy',   'Okafor',     'lucy.okafor@nexussupport.com',     'Junior Support Engineer',   1, '2023-04-03'),
('Marcus', 'Webb',       'marcus.webb@nexussupport.com',     'Network Manager',           2, '2018-11-20'),
('Elena',  'Russo',      'elena.russo@nexussupport.com',     'Senior Network Engineer',   2, '2020-02-14'),
('Danny',  'Choi',       'danny.choi@nexussupport.com',      'Network Engineer',          2, '2021-07-19'),
('Fiona',  'Blake',      'fiona.blake@nexussupport.com',     'Network Engineer',          2, '2022-05-30'),
('Ahmed',  'Hassan',     'ahmed.hassan@nexussupport.com',    'Security Manager',          3, '2017-08-07'),
('Claire', 'Sutton',     'claire.sutton@nexussupport.com',   'Senior Security Analyst',   3, '2019-12-01'),
('Raj',    'Sharma',     'raj.sharma@nexussupport.com',      'Security Analyst',          3, '2021-03-22'),
('Zoe',    'Fleming',    'zoe.fleming@nexussupport.com',     'Security Analyst',          3, '2022-08-15'),
('Ben',    'Thornton',   'ben.thornton@nexussupport.com',    'IT Manager',                1, '2016-05-11'),
('Nina',   'Castillo',   'nina.castillo@nexussupport.com',   'Support Engineer',          1, '2023-10-02');

INSERT INTO Customers (CompanyName, ContactName, Email, ContractTier, ContractValue) VALUES
('Apex Financial',       'Richard Lawson', 'r.lawson@apexfinancial.com',       'Enterprise', 120000.00),
('BlueSky Logistics',    'Karen Bright',   'k.bright@blueskylogistics.com',    'Premium',     45000.00),
('Cornerstone Legal',    'David Park',     'd.park@cornerstonelegal.com',      'Enterprise',  95000.00),
('Delta Manufacturing',  'Susan Holt',     's.holt@deltamfg.com',              'Standard',    12000.00),
('Echo Retail Group',    'Mike Daniels',   'm.daniels@echoretail.com',         'Premium',     38000.00),
('Frontier Healthcare',  'Amanda Ross',    'a.ross@frontierhealthcare.com',    'Enterprise', 150000.00),
('Granite Construction', 'Paul Yates',     'p.yates@graniteconstruction.com',  'Standard',     8000.00),
('Horizon Media',        'Tina Wallis',    't.wallis@horizonmedia.com',        'Premium',     52000.00),
('Ironclad Security',    'Chris Doyle',    'c.doyle@ironcladsecurity.com',     'Standard',    15000.00),
('Jade Technology',      'Mei Lin',        'm.lin@jadetechnology.com',         'Enterprise', 200000.00);

INSERT INTO Tickets (CustomerId, AssignedEmployeeId, Title, Status, Priority, Category, CreatedDate, ResolvedDate) VALUES
(1,  2,  'Unable to access trading platform after network change',       'Resolved',   'Critical', 'Network',  '2026-01-05 08:23:00', '2026-01-05 11:45:00'),
(1,  11, 'Suspected phishing emails targeting finance team',             'Closed',     'High',     'Security', '2026-01-12 09:10:00', '2026-01-13 14:30:00'),
(1,  3,  'Laptops freezing during end of month reporting',               'Resolved',   'High',     'Hardware', '2026-01-28 10:00:00', '2026-01-29 16:00:00'),
(1,  4,  'New analyst cannot log into reporting software',               'Closed',     'Medium',   'Access',   '2026-02-03 08:45:00', '2026-02-03 10:15:00'),
(1,  7,  'Intermittent VPN dropouts for remote traders',                 'Open',       'Critical', 'Network',  '2026-04-10 07:55:00', NULL),

(2,  8,  'Warehouse barcode scanners losing network connectivity',       'Resolved',   'High',     'Network',  '2026-01-08 11:30:00', '2026-01-09 09:00:00'),
(2,  3,  'Printer in dispatch bay not responding',                       'Closed',     'Low',      'Hardware', '2026-01-20 13:00:00', '2026-01-20 15:30:00'),
(2,  12, 'Unusual login attempts detected on logistics portal',          'Resolved',   'High',     'Security', '2026-02-14 08:00:00', '2026-02-14 17:00:00'),
(2,  5,  'Fleet management software crashes on startup',                 'InProgress', 'Medium',   'Software', '2026-04-01 09:30:00', NULL),
(2,  4,  'New driver onboarding accounts not created',                   'Open',       'Medium',   'Access',   '2026-04-18 10:00:00', NULL),

(3,  11, 'Ransomware alert triggered on partners shared drive',          'Resolved',   'Critical', 'Security', '2026-01-15 06:45:00', '2026-01-15 14:00:00'),
(3,  2,  'Document management system timing out on large files',         'Closed',     'High',     'Software', '2026-01-22 09:00:00', '2026-01-24 11:00:00'),
(3,  7,  'Video conferencing dropping during client calls',              'Resolved',   'Medium',   'Network',  '2026-02-05 10:30:00', '2026-02-06 09:00:00'),
(3,  3,  'Senior partner laptop will not boot',                          'Closed',     'High',     'Hardware', '2026-02-19 08:00:00', '2026-02-19 12:30:00'),
(3,  13, 'Two-factor authentication not working for remote staff',       'InProgress', 'High',     'Security', '2026-04-07 09:15:00', NULL),

(4,  8,  'Factory floor devices cannot reach ERP system',               'Resolved',   'Critical', 'Network',  '2026-01-10 06:00:00', '2026-01-10 10:30:00'),
(4,  5,  'ERP reporting module showing incorrect stock figures',         'Closed',     'Medium',   'Software', '2026-02-01 09:00:00', '2026-02-03 14:00:00'),
(4,  4,  'New production line staff need system accounts',               'Closed',     'Low',      'Access',   '2026-02-10 10:00:00', '2026-02-10 11:30:00'),
(4,  9,  'Site to site VPN between two factories unstable',              'InProgress', 'High',     'Network',  '2026-03-25 08:00:00', NULL),
(4,  3,  'Workstation in quality control overheating and shutting down', 'Open',       'Medium',   'Hardware', '2026-04-20 11:00:00', NULL),

(5,  2,  'Point of sale terminals offline across three stores',          'Resolved',   'Critical', 'Network',  '2026-01-06 07:30:00', '2026-01-06 09:45:00'),
(5,  12, 'Customer data export flagged by security scan',                'Closed',     'High',     'Security', '2026-01-25 09:00:00', '2026-01-27 16:00:00'),
(5,  5,  'Stock management software not syncing with warehouse',         'Resolved',   'Medium',   'Software', '2026-02-08 10:00:00', '2026-02-09 14:00:00'),
(5,  4,  'Store manager locked out of back office system',               'Closed',     'Medium',   'Access',   '2026-03-01 08:30:00', '2026-03-01 09:00:00'),
(5,  9,  'Store network running very slowly during peak hours',          'Open',       'High',     'Network',  '2026-04-15 13:00:00', NULL),

(6,  11, 'Patient records system unreachable from ward terminals',       'Resolved',   'Critical', 'Network',  '2026-01-03 05:30:00', '2026-01-03 07:15:00'),
(6,  13, 'Possible unauthorised access to patient data portal',          'Resolved',   'Critical', 'Security', '2026-01-18 08:00:00', '2026-01-19 18:00:00'),
(6,  2,  'Medical imaging software crashing on radiology workstations',  'Closed',     'High',     'Software', '2026-02-02 09:00:00', '2026-02-04 11:00:00'),
(6,  3,  'Several ward tablets have cracked screens and dead batteries', 'InProgress', 'Low',      'Hardware', '2026-03-10 10:00:00', NULL),
(6,  4,  'Locum doctors unable to access prescribing system',            'Open',       'High',     'Access',   '2026-04-22 08:00:00', NULL),

(7,  8,  'Site office has no internet since router was replaced',        'Resolved',   'High',     'Network',  '2026-01-14 09:00:00', '2026-01-14 14:00:00'),
(7,  5,  'Project management software licence expired',                  'Closed',     'Medium',   'Software', '2026-02-06 10:00:00', '2026-02-06 11:00:00'),
(7,  3,  'Rugged laptop used on site has failed display',                'Closed',     'Medium',   'Hardware', '2026-02-20 08:30:00', '2026-02-21 12:00:00'),
(7,  15, 'Site foreman needs access to supplier portal',                 'Closed',     'Low',      'Access',   '2026-03-05 09:00:00', '2026-03-05 10:00:00'),
(7,  9,  'CCTV system on construction site cannot connect to HQ',        'Open',       'Medium',   'Network',  '2026-04-25 11:00:00', NULL),

(8,  7,  'Live broadcast network experiencing packet loss',              'Resolved',   'Critical', 'Network',  '2026-01-09 04:00:00', '2026-01-09 06:30:00'),
(8,  12, 'Unauthorised device detected on production network',           'Resolved',   'High',     'Security', '2026-01-30 09:00:00', '2026-01-30 17:00:00'),
(8,  5,  'Video editing software not recognising GPU after update',      'Closed',     'High',     'Software', '2026-02-12 10:00:00', '2026-02-13 15:00:00'),
(8,  3,  'Editing suite workstation making loud grinding noise',         'Resolved',   'Medium',   'Hardware', '2026-02-25 09:00:00', '2026-02-26 11:00:00'),
(8,  15, 'Freelance editors need temporary VPN access',                  'InProgress', 'Medium',   'Access',   '2026-04-05 10:00:00', NULL),

(9,  11, 'Firewall rules blocking legitimate client traffic',            'Resolved',   'High',     'Security', '2026-01-11 08:00:00', '2026-01-11 13:00:00'),
(9,  7,  'Network monitoring tool showing false positive alerts',        'Closed',     'Medium',   'Network',  '2026-01-26 09:00:00', '2026-01-28 11:00:00'),
(9,  5,  'Vulnerability scanner licence needs renewal',                  'Closed',     'Low',      'Software', '2026-02-15 10:00:00', '2026-02-15 11:30:00'),
(9,  4,  'New security consultant needs admin access provisioned',       'Closed',     'Medium',   'Access',   '2026-03-08 09:00:00', '2026-03-08 10:30:00'),
(9,  13, 'Suspected brute force attack on client VPN gateway',           'Open',       'Critical', 'Security', '2026-04-28 02:00:00', NULL),

(10, 2,  'Development environment network completely unreachable',       'Resolved',   'Critical', 'Network',  '2026-01-07 07:00:00', '2026-01-07 09:30:00'),
(10, 13, 'Source code repository access logs show anomalous activity',   'Resolved',   'High',     'Security', '2026-01-21 09:00:00', '2026-01-22 16:00:00'),
(10, 5,  'CI/CD pipeline failing after server OS update',                'Closed',     'High',     'Software', '2026-02-17 10:00:00', '2026-02-18 14:00:00'),
(10, 3,  'Developer workstations need RAM upgrade for new tooling',      'InProgress', 'Medium',   'Hardware', '2026-03-20 09:00:00', NULL),
(10, 15, 'Contractors need access to staging environment',               'Open',       'Medium',   'Access',   '2026-04-30 10:00:00', NULL);
