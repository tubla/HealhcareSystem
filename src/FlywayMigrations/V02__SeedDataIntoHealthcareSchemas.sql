
-- medical records
create table [healthcare].[medical_record] (
    record_id int identity(1,1) primary key,
    patient_id int not null,
    doctor_id int not null,
    diagnosis nvarchar(200) not null,
    treatment nvarchar(200),
    record_date date not null default getdate(),
    foreign key (patient_id) references [healthcare].[patient](patient_id),
    foreign key (doctor_id) references [healthcare].[doctor](doctor_id));

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
    ('admin', 'full system access'),
    ('doctor', 'access to clinical data'),
    ('nurse', 'manage appointments'),
    ('billing', 'handle billing'),
    ('patient', 'view own data'),
    ('inventory', 'manage medications');

go

--permission
insert into [healthcare].[permission]([permission_name],[description])
values
    ('readpatient', 'view patient details'),
    ('writepatient', 'update patient details'),
    ('readdoctor', 'view doctor details'),
    ('writedoctor', 'update doctor details'),
    ('readappointment', 'view appointments'),
    ('writeappointment', 'create/update appointments'),
    ('cancelappointment', 'cancel appointments'),
    ('writeprescription', 'issue prescriptions'),
    ('readbill', 'view bills'),
    ('writebill', 'create/update bills'),
    ('readmedicalrecord', 'view medical records'),
    ('writemedicalrecord', 'create/update medical records'),
    ('readmedication', 'view medications'),
    ('writemedication', 'update medications'),
    ('readsupplier', 'view suppliers'),
    ('writesupplier', 'update suppliers');

go

--user_role
insert into [healthcare].[user_role](user_id, role_id)
select u.user_id, r.role_id 
from [healthcare].[user] u
inner join [healthcare].[role] r on 
    (u.user_name = 'admin' and r.role_name = 'admin') or
    (u.user_name = 'dr_smith' and r.role_name = 'doctor') or
    (u.user_name = 'nurse_jane' and r.role_name = 'nurse') or
    (u.user_name = 'billing_staff' and r.role_name = 'billing') or
    (u.user_name = 'alice_brown' and r.role_name = 'patient') or
    (u.user_name = 'inventory_mgr' and r.role_name = 'inventory');

go

--role_permission
insert into [healthcare].[role_permission](role_id, permission_id)
select r.role_id, p.permission_id 
from [healthcare].[role] r
inner join [healthcare].[permission] p on
    (r.role_name = 'admin' and p.permission_name in ('readpatient', 'writepatient', 'readdoctor', 'writedoctor', 'readappointment', 'writeappointment', 'cancelappointment', 'writeprescription', 'readbill', 'writebill', 'readmedicalrecord', 'writemedicalrecord', 'readmedication', 'writemedication', 'readsupplier', 'writesupplier')) or
    (r.role_name = 'doctor' and p.permission_name in ('readpatient', 'readappointment', 'writeappointment', 'writeprescription', 'readmedicalrecord', 'writemedicalrecord')) or
    (r.role_name = 'nurse' and p.permission_name in ('readpatient', 'readappointment', 'writeappointment')) or
    (r.role_name = 'billing' and p.permission_name in ('readpatient', 'readbill', 'writebill')) or
    (r.role_name = 'patient' and p.permission_name in ('readpatient', 'readmedicalrecord')) or
    (r.role_name = 'inventory' and p.permission_name in ('readmedication', 'writemedication', 'readsupplier', 'writesupplier'));

go

--supplier
insert into [healthcare].[supplier]([name],contact)
values
    ('pharmacorp', 'supply@pharmacorp.com'), 
    ('medisupply', 'sales@medisupply.com');

go

--department
insert into [healthcare].[department]([name])
values
    ('cardiology'), 
    ('pediatrics'), 
    ('neurology');

go

--insurance_provider
insert into [healthcare].[insurance_provider]([name],contact)
values
    ('healthcare inc.', 'contact@healthcare.com'), 
    ('medisure', 'info@medisure.com');

go

--patient
insert into [healthcare].[patient]([name],dob,gender,email,phone,[address],insurance_provider_id,[user_id])
select 'alice brown', '1985-03-15', 'f', 'alice.brown@example.com', '9878765665', 'kolding, poland', ip.insurance_provider_id, u.user_id
from [healthcare].[insurance_provider] ip
left join [healthcare].[user] u on u.user_name = 'alice_brown'
where ip.name = 'healthcare inc.'
union all
select 'bob white', '1990-07-22', 'm', 'bob.white@example.com', '8987678445', 'grenoble, france', ip.insurance_provider_id, null
from [healthcare].[insurance_provider] ip
where ip.name = 'medisure';

go

--doctor
insert into [healthcare].[doctor]([name],license_number,specialization,email,phone,dept_id,[user_id])
select 'dr. john smith', 'lic12345', 'cardiologist', 'john.smith@example.com', '6565787665', d.dept_id, u.user_id
from [healthcare].[department] d
left join [healthcare].[user] u on u.user_name = 'dr_smith'
where d.name = 'cardiology'
union all
select 'dr. emily davis', 'lic67890', 'pediatrician', 'emily.davis@example.com', '8767656556', d.dept_id, null
from [healthcare].[department] d
where d.name = 'pediatrics';

go

--appointment
insert into [healthcare].[appointment](patient_id,doctor_id,appointment_date_time,[status],reason)
select p.patient_id, d.doctor_id, '2025-06-15 10:00', 'scheduled', 'heart checkup'
from [healthcare].[patient] p
inner join [healthcare].[doctor] d on p.email = 'alice.brown@example.com' and d.email = 'john.smith@example.com'
union all
select p.patient_id, d.doctor_id, '2025-06-16 14:00', 'completed', 'child wellness'
from [healthcare].[patient] p
inner join [healthcare].[doctor] d on p.email = 'bob.white@example.com' and d.email = 'emily.davis@example.com';

go

--medication
insert into [healthcare].[medication]([name],[type],stock_quantity,unit_cost,supplier_id)
select 'aspirin', 'tablet', 100, 0.50, s.supplier_id
from [healthcare].[supplier] s
where s.name = 'pharmacorp'
union all
select 'amoxicillin', 'capsule', 50, 1.20, s.supplier_id
from [healthcare].[supplier] s
where s.name = 'medisupply';

go

--prescription
insert into [healthcare].[prescription](appointment_id,medication_id,dosage,duration,issue_date)
select a.appointment_id, m.medication_id, '1 tablet daily', '7 days', '2025-06-15'
from [healthcare].[appointment] a
inner join [healthcare].[medication] m on a.reason = 'heart checkup' and m.name = 'aspirin'
union all
select a.appointment_id, m.medication_id, '1 capsule twice daily', '5 days', '2025-06-16'
from [healthcare].[appointment] a
inner join [healthcare].[medication] m on a.reason = 'child wellness' and m.name = 'amoxicillin';

go

--bill
insert into [healthcare].[bill](appointment_id,patient_id,total_amount,insurance_coverage,payment_status,issue_date)
select a.appointment_id, p.patient_id, 150.00, 120.00, 'partial', '2025-06-15'
from [healthcare].[appointment] a
inner join [healthcare].[patient] p on a.reason = 'heart checkup' and p.email = 'alice.brown@example.com'
union all
select a.appointment_id, p.patient_id, 100.00, 80.00, 'pending', '2025-06-16'
from [healthcare].[appointment] a
inner join [healthcare].[patient] p on a.reason = 'child wellness' and p.email = 'bob.white@example.com';

go

--medical_record
insert into [healthcare].[medical_record] (patient_id,doctor_id,diagnosis,treatment,record_date)
select p.patient_id, d.doctor_id, 'hypertension', 'prescribed aspirin', '2025-06-15'
from [healthcare].[patient] p
inner join [healthcare].[doctor] d on p.email = 'alice.brown@example.com' and d.email = 'john.smith@example.com'
union all
select p.patient_id, d.doctor_id, 'ear infection', 'prescribed amoxicillin', '2025-06-16'
from [healthcare].[patient] p
inner join [healthcare].[doctor] d on p.email = 'bob.white@example.com' and d.email = 'emily.davis@example.com';

go
