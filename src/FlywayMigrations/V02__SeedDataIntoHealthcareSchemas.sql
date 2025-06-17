
-- medical records
create table medical_record (
    record_id int identity(1,1) primary key,
    patient_id int not null,
    doctor_id int not null,
    diagnosis nvarchar(200) not null,
    treatment nvarchar(200),
    record_date date not null default getdate(),
    foreign key (patient_id) references patient(patient_id),
    foreign key (doctor_id) references doctor(doctor_id));

go

--user
insert into [healthcare].[user] ([user_name], password_hash, email)
values 
    ('admin', 'hashed_password_admin', 'admin@healthcare.com'),
    ('dr_smith', 'hashed_password_smith', 'john.smith@example.com'),
    ('nurse_jane', 'hashed_password_jane', 'jane.doe@example.com'),
    ('billing_staff', 'hashed_password_billing', 'billing@healthcare.com'),
    ('alice_brown', 'hashed_password_alice', 'alice.brown@example.com'),
    ('inventory_mgr', 'hashed_password_inventory', 'inventory@healthcare.com');

go

--role
insert into [healthcare].[role](role_name,[description])
values
	('Admin', 'Full system access'),
    ('Doctor', 'Access to clinical data'),
    ('Nurse', 'Manage appointments'),
    ('Billing', 'Handle billing'),
    ('Patient', 'View own data'),
    ('Inventory', 'Manage medications');

go

--permission

insert into [healthcare].[permission]([permission_name],[description])
values
	('ReadPatient', 'View patient details'),
    ('WritePatient', 'Update patient details'),
    ('ReadDoctor', 'View doctor details'),
    ('WriteDoctor', 'Update doctor details'),
    ('ReadAppointment', 'View appointments'),
    ('WriteAppointment', 'Create/update appointments'),
    ('CancelAppointment', 'Cancel appointments'),
    ('WritePrescription', 'Issue prescriptions'),
    ('ReadBill', 'View bills'),
    ('WriteBill', 'Create/update bills'),
    ('ReadMedicalRecord', 'View medical records'),
    ('WriteMedicalRecord', 'Create/update medical records'),
    ('ReadMedication', 'View medications'),
    ('WriteMedication', 'Update medications'),
    ('ReadSupplier', 'View suppliers'),
    ('WriteSupplier', 'Update suppliers');

go

-- user_role
insert into [healthcare].[user_role]([user_id],role_id)
values
	(1, 1), -- Admin
    (2, 2), -- Doctor
    (3, 3), -- Nurse
    (4, 4), -- Billing
    (5, 5), -- Patient
    (6, 6); -- Inventory

go

-- role_permission
insert into [healthcare].[role_permission](role_id,permission_id)
values
	(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8), (1, 9), (1, 10), (1, 11), (1, 12), (1, 13), (1, 14), (1, 15), (1, 16), -- Admin: All permissions
    (2, 1), (2, 5), (2, 6), (2, 8), (2, 11), (2, 12), -- Doctor: ReadPatient, Read/WriteAppointment, WritePrescription, Read/WriteMedicalRecord
    (3, 1), (3, 5), (3, 6), -- Nurse: ReadPatient, Read/WriteAppointment
    (4, 1), (4, 9), (4, 10), -- Billing: ReadPatient, Read/WriteBill
    (5, 1), (5, 11), -- Patient: ReadPatient, ReadMedicalRecord (own data only)
    (6, 13), (6, 14), (6, 15), (6, 16); -- Inventory: Read/WriteMedication, Read/WriteSupplier

go

-- supplier
insert into [healthcare].[supplier]([name],contact)
values
	('PharmaCorp', 'supply@pharmacorp.com'), 
	('MediSupply', 'sales@medisupply.com');

go

--department
insert into [healthcare].[department]([name])
values
	('Cardiology'), 
	('Pediatrics'), 
	('Neurology');

go

--insurance_provider
insert into [healthcare].[insurance_provider]([name],contact)
values
	('HealthCare Inc.', 'contact@healthcare.com'), 
	('MediSure', 'info@medisure.com');

go

-- patient
insert into [healthcare].[patient]([name],dob,gender,email,phone,[address],insurance_provider_id,[user_id])
values
	('Alice Brown', '1985-03-15', 'F', 'alice.brown@example.com', '9878765665', 1, 5),
    ('Bob White', '1990-07-22', 'M', 'bob.white@example.com', '8987678445', 2, NULL);

go

-- doctor
insert into [healthcare].[doctor]([name],license_number,specialization,email,phone,dept_id,[user_id])
values
	('Dr. John Smith', 'LIC12345', 'Cardiologist', 'john.smith@example.com', 1, 2),
    ('Dr. Emily Davis', 'LIC67890', 'Pediatrician', 'emily.davis@example.com', 2, NULL);

go

-- appointment
insert into [healthcare].[appointment](patient_id,doctor_id,appointment_date_time,[status],reason)
values
	(1, 1, '2025-06-15 10:00', 'Scheduled', 'Heart checkup'),
    (2, 2, '2025-06-16 14:00', 'Completed', 'Child wellness');

go

-- medication
insert into [healthcare].[medication]([name],[type],stock_quantity,unit_cost,supplier_id)
values
	('Aspirin', 'Tablet', 100, 0.50, 1),
    ('Amoxicillin', 'Capsule', 50, 1.20, 2);

go

-- prescription
insert into [healthcare].[prescription](appointment_id,medication_id,dosage,duration,issue_date)
values
	(1, 1, '1 tablet daily', '7 days', '2025-06-15'),
    (2, 2, '1 capsule twice daily', '5 days', '2025-06-16');

go

-- bill
insert into [healthcare].[bill](appointment_id,patient_id,total_amount,insurance_coverage,payment_status,issue_date)
values
	(1, 1, 150.00, 120.00, 'Partial', '2025-06-15'),
    (2, 2, 100.00, 80.00, 'Pending', '2025-06-16');

go

-- medical records
insert into medical_record (patient_id,doctor_id,diagnosis,treatment,record_date)
values
	(1, 1, 'Hypertension', 'Prescribed Aspirin', '2025-06-15'),
    (2, 2, 'Ear infection', 'Prescribed Amoxicillin', '2025-06-16');

go
