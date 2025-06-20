
-- media
create table [healthcare].[media] (
    media_id int identity(1,1) primary key,
    file_name nvarchar(100) not null,
    file_url nvarchar(500) not null,
    upload_date date not null default getdate(),
    content_type nvarchar(100) not null);

go

-- prescription_media
create table [healthcare].[prescription_media] (
	prescription_id int not null,
    media_id int not null,
    primary key(prescription_id, media_id),
    foreign key(prescription_id) references [healthcare].[prescription](prescription_id) on delete cascade,
    foreign key(media_id) references [healthcare].[media](media_id) on delete cascade);

go
