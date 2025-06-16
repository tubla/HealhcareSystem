
--user
create table [healthcare].[user](
[user_id] int identity(1,1) primary key,
[user_name] nvarchar(100) not null unique,
password_hash nvarchar(500) not null,
email nvarchar(100) unique);

go

--role
create table [healthcare].[role](
role_id int identity(1,1) primary key,
role_name nvarchar(100) not null unique,
[description] nvarchar(300));

go

--permission

create table [healthcare].[permission](
permission_id int identity(1,1) primary key,
[permission_name] nvarchar(200) unique not null,
[description] nvarchar(300));

go

-- user_role
create table [healthcare].[user_role](
[user_id] int not null,
role_id int not null,
primary key([user_id],role_id),
foreign key([user_id]) references [healthcare].[user]([user_id]) on delete cascade,
foreign key([role_id]) references [healthcare].[role](role_id) on delete cascade);

go

-- role_permission
create table [healthcare].[role_permission](
role_id int not null,
permission_id int not null,
primary key(role_id,permission_id),
foreign key(role_id) references [healthcare].[role](role_id) on delete cascade,
foreign key(permission_id) references [healthcare].[permission](permission_id) on delete cascade);

go

-- supplier
create table [healthcare].[supplier](
supplier_id int identity(1,1) primary key,
[name] nvarchar(100) not null,
contact nvarchar(200));

go

--department
create table [healthcare].[department](
dept_id int identity(1,1) primary key,
[name] nvarchar(100) not null);

go

--insurance_provider
create table [healthcare].[insurance_provider](
insurance_provider_id int identity(1,1) primary key,
[name] nvarchar(100) not null,
contact nvarchar(200));

go

-- patient
create table [healthcare].[patient](
patient_id int identity(1,1) primary key,
[name] nvarchar(100) not null,
dob date not null check(dob >= dateadd(year,-120,getdate()) and dob <= getdate()),
gender char(1) check(gender in('M','F','O')),
email nvarchar(100) unique not null,
phone varchar(10) not null,
[address] nvarchar(300),
insurance_provider_id int,
[user_id] int unique,
foreign key(insurance_provider_id) references [healthcare].insurance_provider(insurance_provider_id),
foreign key([user_id]) references [healthcare].[user]([user_id]));

go

-- doctor
create table [healthcare].[doctor](
doctor_id int identity(1,1) primary key,
[name] nvarchar(100) not null,
license_number nvarchar(50) unique not null,
specialization nvarchar(100) not null,
email nvarchar(100) unique not null,
phone varchar(10) not null,
dept_id int not null,
[user_id] int unique,
foreign key(dept_id) references [healthcare].department(dept_id),
foreign key([user_id]) references [healthcare].[user]([user_id]));

go

-- appointment
create table [healthcare].[appointment](
appointment_id int identity(1,1) primary key,
patient_id int unique not null,
doctor_id int unique not null,
appointment_date_time datetime not null,
[status] nvarchar(20) check([status] in('Scheduled','Completed','Cancelled')),
reason nvarchar(100),
foreign key(patient_id) references [healthcare].patient(patient_id),
foreign key(doctor_id) references [healthcare].doctor(doctor_id),
constraint uk_appointment unique(doctor_id,appointment_date_time)); -- Important

go

-- medication
create table [healthcare].[medication](
medication_id int identity(1,1) primary key,
[name] nvarchar(100) unique not null,
[type] nvarchar(50),
stock_quantity int check(stock_quantity >= 0),
unit_cost decimal(10,2) not null, -- A number with up to 10 digits total, 2 of which are after the decimal point
supplier_id int not null,
foreign key(supplier_id) references [healthcare].supplier(supplier_id));

go

-- prescription
create table [healthcare].[prescription](
prescription_id int identity(1,1) primary key,
appointment_id int not null,
medication_id int not null,
dosage nvarchar(100) not null,
duration nvarchar(50) not null,
issue_date date not null default getdate(),
foreign key(appointment_id) references [healthcare].appointment(appointment_id),
foreign key(medication_id) references [healthcare].medication(medication_id));

go

-- bill
create table [healthcare].[bill](
bill_id int identity(1,1) primary key,
appointment_id int,
patient_id int not null,
total_amount decimal(10,2) not null,
insurance_coverage decimal(10,2) default 0,
payment_status nvarchar(20) not null check(payment_status in ('Pending','Paid','Partial')),
issue_date date not null default getdate(),
foreign key(appointment_id) references [healthcare].appointment(appointment_id),
foreign key(patient_id) references [healthcare].patient(patient_id));

go
