
-- doctor_slot_availability
insert into [healthcare].[doctor_slot_availability] (doctor_id, slot_date, appointment_count, slot_status)
select d.doctor_id, '2025-06-28', 1, 'Available'
from [healthcare].[doctor] d
where d.email = 'john.smith@example.com'
union all
select d.doctor_id, '2025-06-28', 1, 'Available'
from [healthcare].[doctor] d
where d.email = 'emily.davis@example.com'
union all
select d.doctor_id, '2025-06-29', 0, 'Available'
from [healthcare].[doctor] d
where d.email = 'john.smith@example.com'
union all
select d.doctor_id, '2025-06-29', 0, 'Available'
from [healthcare].[doctor] d
where d.email = 'emily.davis@example.com';

go
