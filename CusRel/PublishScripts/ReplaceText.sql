SET NOCOUNT ON
DECLARE @ObjectsCount INT, @Counter INT, @SqlToRun	Varchar(MAX), @oID	Int, @name varchar(max)
DECLARE @objects Table(oid int, name varchar(max), ID int identity(1,1))
DECLARE @Result Table (oid int, name varchar(max), content	varchar(Max), OrderNum int )
DECLARE @Temp	table(content varchar(max),ID int identity(1,1))
DECLARE @searchFor VARCHAR(max), @replaceWith VARCHAR(max)
DECLARE @searchFor1 VARCHAR(max), @replaceWith1 VARCHAR(max)
DECLARE @searchFor2 VARCHAR(max), @replaceWith2 VARCHAR(max)
DECLARE @searchFor3 VARCHAR(max), @replaceWith3 VARCHAR(max)
DECLARE @OutputFolder varchar(max) = 'G:\deleteme\queries\'
SET @searchFor = '[EmployeeDW].'
SET @replaceWith = '[@ReplaceWithEmployeeDW].'
SET @searchFor1 = '[SchedulingDW].'
SET @replaceWith1 = '[@ReplaceWithSchedulingDW].'
SET @searchFor2 = '[MaintenanceDW].'
SET @replaceWith2 = '[@ReplaceWithMaintenanceDW].'
SET @searchFor3 = '[$(your.SQL.serverProd01)].EmployeeDW.'
SET @replaceWith3 = '[@ReplaceWithEmployeeDW].'

insert into @objects
SELECT DISTINCT o.id as oid, QUOTENAME(OBJECT_SCHEMA_NAME(c.id))+'.'+QUOTENAME(OBJECT_NAME(c.id))
FROM syscomments c 
INNER JOIN sysobjects o ON c.id=o.id
WHERE o.xtype not in ('D','F','C','AF','L','FS','FT','IT','PC','PK','RF','S','SN','SQ','TA','TT','U','UQ')
    AND OBJECT_NAME(c.id) not like 'sp_%'
    AND OBJECT_NAME(c.id) not like 'fn_%'
    AND OBJECT_NAME(c.id) not like '_Temp%'
ORDER BY QUOTENAME(OBJECT_SCHEMA_NAME(c.id))+'.'+QUOTENAME(OBJECT_NAME(c.id))

SELECT @ObjectsCount = MAX(ID) FROM @objects
SET @Counter = 0

While @Counter<@ObjectsCount
BEGIN
	SET @Counter = @Counter + 1
	select @Oid= oid, @name = name from @objects where id=@Counter
	set @sqlToRun = 'sp_helptext ''' + @name + ''''

	delete from @Temp
	Insert @Temp
	Exec (@SqltoRun)

   UPDATE @Temp SET content = REPLACE(content, 'CREATE PROCEDURE ', 'ALTER PROCEDURE ')
   UPDATE @Temp SET content = REPLACE(content, 'CREATE PROC ', 'ALTER PROC ')
   UPDATE @Temp SET content = REPLACE(content, 'CREATE FUNCTION ', 'ALTER FUNCTION ')
   UPDATE @Temp SET content = REPLACE(content, 'CREATE FUNC ', 'ALTER FUNC ')
   UPDATE @Temp SET content = REPLACE(content, 'CREATE VIEW ', 'ALTER VIEW ')
   UPDATE @Temp SET content = REPLACE(content, 'CREATE TRIGGER ', 'ALTER TRIGGER ')
   UPDATE @Temp SET content = REPLACE(content, @searchFor,@replaceWith)
   UPDATE @Temp SET content = REPLACE(content, @searchFor1,@replaceWith1)
   UPDATE @Temp SET content = REPLACE(content, @searchFor2,@replaceWith2)
   UPDATE @Temp SET content = REPLACE(content, @searchFor3,@replaceWith3)

	Insert into @Result (oid, name, content, OrderNum)
	Select @oid, @name, content, ID from @Temp
END

DECLARE @TempResult Table (oid int, name varchar(max), text	varchar(Max), Id int Identity(1,1))
Insert @TempResult
Select * FROM (
	SELECT R2.oid,R2.name, 
		REPLACE(
			REPLACE(
				REPLACE(
					STUFF(
					(			
						Select '' + content AS [text()]
						From @Result
						Where (oid = R2.oid)
						ORDER BY OrderNum            
						For XML PATH (''))
						, 1, 0,'')
					--, 1, 0,'Use Training' + + char(10) + char(13) + 'GO' + char(10) + char(13))
					, '&#x0D;',char(13)) 
				,'&gt;','>')
			,'&lt;','<')
			text
	FROM @Result R2
	group by R2.oid, R2.name
) X 
Where Charindex(@replaceWith,X.text,0)>0
	or Charindex(@replaceWith1,X.text,0)>0
	or Charindex(@replaceWith2,X.text,0)>0 

--select * from @TempResult 
--where Charindex('$(',text,0)>0 
--	or Charindex('&',text,0)>0 

--return


Select @ObjectsCount=Max(id) from @TempResult
set @Counter = 0
while (@Counter<@ObjectsCount)
Begin
	set @Counter = @Counter + 1

	DECLARE @Text AS NVARCHAR(MAX)
	DECLARE @Cmd AS VARCHAR(MAX)
	SELECT @Text = Text, @name= name From @TempResult Where ID=@Counter
	--SET @Text = REPLACE(@Text, '|', '^|')
	--SET @Text = REPLACE(@Text, '''', '''''')
	--set @Text = LTrim(RTRIM(@Text))
	--select  @text
	execute sp_executesql @statement = @text
	--execute @Text
	--SET @Cmd ='echo "' +  @Text + '" > ' + @OutputFolder + @name + '.txt'	
	--SET @Text = 'Hello world^| '
	--SET @Cmd ='echo ' + @Text + ' > ' + @OutputFolder + @name + '.txt'	
	--SET @Cmd ='echo ' +  @Text + ' > C:\AppTextFile.txt'
	--select @Cmd
	--EXECUTE xp_CmdShell  @Cmd
	--return
End




--xp_cmdshell 'dir'


--sp_configure 'show advanced options',1
--reconfigure
--Go
--sp_configure 'xp_cmdshell',1
--reconfigure
--Go
--xp_cmdshell 'dir'
--sp_configure 'xp_cmdshell',0
--reconfigure
--Go
--sp_configure 'show advanced options',0
--reconfigure
--Go

