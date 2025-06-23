
-- doctor_slot_availability
create table [healthcare].[doctor_slot_availability](
    slot_id int identity(1,1) primary key,
    doctor_id int not null,
    slot_date date not null,
    appointment_count int not null default 0,
    slot_status nvarchar(20) not null default 'Available',
    row_version rowversion not null,
    constraint CHK_slot_status check (slot_status in ('Available', 'Full')),
    constraint UQ_doctor_slot_date unique (doctor_id, slot_date),
    constraint FK_doctor_slot_doctor foreign key (doctor_id) references [healthcare].[doctor](doctor_id)
);

go
