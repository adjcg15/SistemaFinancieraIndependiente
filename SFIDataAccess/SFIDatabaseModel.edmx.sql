
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/17/2024 04:21:11
-- Generated from EDMX file: C:\Users\dnava\Desktop\Repo\SFIDataAccess\SFIDatabaseModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [financiera_independiente];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_clients_addresses]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[clients] DROP CONSTRAINT [FK_clients_addresses];
GO
IF OBJECT_ID(N'[dbo].[FK_clients_bank_accounts]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[clients] DROP CONSTRAINT [FK_clients_bank_accounts];
GO
IF OBJECT_ID(N'[dbo].[FK_clients_work_centers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[clients] DROP CONSTRAINT [FK_clients_work_centers];
GO
IF OBJECT_ID(N'[dbo].[FK_contact_methods_clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[contact_methods] DROP CONSTRAINT [FK_contact_methods_clients];
GO
IF OBJECT_ID(N'[dbo].[FK_contact_methods_contact_method_types]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[contact_methods] DROP CONSTRAINT [FK_contact_methods_contact_method_types];
GO
IF OBJECT_ID(N'[dbo].[FK_contact_methods_personal_references]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[contact_methods] DROP CONSTRAINT [FK_contact_methods_personal_references];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_applications_clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_applications] DROP CONSTRAINT [FK_credit_applications_clients];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_applications_credit_applications_state]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_applications] DROP CONSTRAINT [FK_credit_applications_credit_applications_state];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_applications_credit_conditions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_applications] DROP CONSTRAINT [FK_credit_applications_credit_conditions];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_applications_credit_types]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_applications] DROP CONSTRAINT [FK_credit_applications_credit_types];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_applications_credits]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_applications] DROP CONSTRAINT [FK_credit_applications_credits];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_applications_dictums]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_applications] DROP CONSTRAINT [FK_credit_applications_dictums];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_applications_system_accounts]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_applications] DROP CONSTRAINT [FK_credit_applications_system_accounts];
GO
IF OBJECT_ID(N'[dbo].[FK_credit_conditions_credit_types]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credit_conditions] DROP CONSTRAINT [FK_credit_conditions_credit_types];
GO
IF OBJECT_ID(N'[dbo].[FK_credits_clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credits] DROP CONSTRAINT [FK_credits_clients];
GO
IF OBJECT_ID(N'[dbo].[FK_credits_credit_types]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[credits] DROP CONSTRAINT [FK_credits_credit_types];
GO
IF OBJECT_ID(N'[dbo].[FK_dictums_system_accounts]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[dictums] DROP CONSTRAINT [FK_dictums_system_accounts];
GO
IF OBJECT_ID(N'[dbo].[FK_digital_documents_credit_applications]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[digital_documents] DROP CONSTRAINT [FK_digital_documents_credit_applications];
GO
IF OBJECT_ID(N'[dbo].[FK_personal_references_addresses]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[personal_references] DROP CONSTRAINT [FK_personal_references_addresses];
GO
IF OBJECT_ID(N'[dbo].[FK_personal_references_clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[personal_references] DROP CONSTRAINT [FK_personal_references_clients];
GO
IF OBJECT_ID(N'[dbo].[FK_polices_apply_dictums_credit_granting_polices]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[polices_apply_dictums] DROP CONSTRAINT [FK_polices_apply_dictums_credit_granting_polices];
GO
IF OBJECT_ID(N'[dbo].[FK_polices_apply_dictums_dictums]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[polices_apply_dictums] DROP CONSTRAINT [FK_polices_apply_dictums_dictums];
GO
IF OBJECT_ID(N'[dbo].[FK_regimes_credit_conditions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[regimes] DROP CONSTRAINT [FK_regimes_credit_conditions];
GO
IF OBJECT_ID(N'[dbo].[FK_regimes_credits]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[regimes] DROP CONSTRAINT [FK_regimes_credits];
GO
IF OBJECT_ID(N'[dbo].[FK_system_accounts_employee_roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[system_accounts] DROP CONSTRAINT [FK_system_accounts_employee_roles];
GO
IF OBJECT_ID(N'[dbo].[FK_work_centers_addresses]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[work_centers] DROP CONSTRAINT [FK_work_centers_addresses];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[addresses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[addresses];
GO
IF OBJECT_ID(N'[dbo].[bank_accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[bank_accounts];
GO
IF OBJECT_ID(N'[dbo].[clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[clients];
GO
IF OBJECT_ID(N'[dbo].[contact_method_types]', 'U') IS NOT NULL
    DROP TABLE [dbo].[contact_method_types];
GO
IF OBJECT_ID(N'[dbo].[contact_methods]', 'U') IS NOT NULL
    DROP TABLE [dbo].[contact_methods];
GO
IF OBJECT_ID(N'[dbo].[credit_applications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[credit_applications];
GO
IF OBJECT_ID(N'[dbo].[credit_applications_state]', 'U') IS NOT NULL
    DROP TABLE [dbo].[credit_applications_state];
GO
IF OBJECT_ID(N'[dbo].[credit_conditions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[credit_conditions];
GO
IF OBJECT_ID(N'[dbo].[credit_granting_polices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[credit_granting_polices];
GO
IF OBJECT_ID(N'[dbo].[credit_types]', 'U') IS NOT NULL
    DROP TABLE [dbo].[credit_types];
GO
IF OBJECT_ID(N'[dbo].[credits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[credits];
GO
IF OBJECT_ID(N'[dbo].[dictums]', 'U') IS NOT NULL
    DROP TABLE [dbo].[dictums];
GO
IF OBJECT_ID(N'[dbo].[digital_documents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[digital_documents];
GO
IF OBJECT_ID(N'[dbo].[employee_roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[employee_roles];
GO
IF OBJECT_ID(N'[dbo].[personal_references]', 'U') IS NOT NULL
    DROP TABLE [dbo].[personal_references];
GO
IF OBJECT_ID(N'[dbo].[polices_apply_dictums]', 'U') IS NOT NULL
    DROP TABLE [dbo].[polices_apply_dictums];
GO
IF OBJECT_ID(N'[dbo].[regimes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[regimes];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[system_accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[system_accounts];
GO
IF OBJECT_ID(N'[dbo].[work_centers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[work_centers];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'addresses'
CREATE TABLE [dbo].[addresses] (
    [id_address] int IDENTITY(1,1) NOT NULL,
    [street] nvarchar(60)  NOT NULL,
    [post_code] char(5)  NOT NULL,
    [neighborhod] nvarchar(60)  NOT NULL,
    [municipality] nvarchar(60)  NOT NULL,
    [outdoor_number] char(10)  NOT NULL,
    [state] nvarchar(60)  NOT NULL,
    [inteior_number] char(10)  NOT NULL,
    [city] nvarchar(60)  NOT NULL
);
GO

-- Creating table 'bank_accounts'
CREATE TABLE [dbo].[bank_accounts] (
    [card_number] char(16)  NOT NULL,
    [bank] nchar(10)  NOT NULL,
    [holder] varchar(255)  NOT NULL
);
GO

-- Creating table 'clients'
CREATE TABLE [dbo].[clients] (
    [rfc] varchar(13)  NOT NULL,
    [name] nvarchar(60)  NOT NULL,
    [surname] nvarchar(60)  NULL,
    [last_name] nvarchar(60)  NOT NULL,
    [curp] char(18)  NOT NULL,
    [birthdate] datetime  NOT NULL,
    [id_address] int  NOT NULL,
    [id_work_center] int  NOT NULL,
    [card_number] char(16)  NOT NULL
);
GO

-- Creating table 'contact_method_types'
CREATE TABLE [dbo].[contact_method_types] (
    [name] nvarchar(50)  NOT NULL,
    [id_contact_method_type] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'contact_methods'
CREATE TABLE [dbo].[contact_methods] (
    [id_contact_method] int IDENTITY(1,1) NOT NULL,
    [value] nvarchar(60)  NOT NULL,
    [id_contact_method_type] int  NOT NULL,
    [client_rfc] varchar(13)  NOT NULL,
    [ine_key] char(18)  NOT NULL
);
GO

-- Creating table 'credit_applications'
CREATE TABLE [dbo].[credit_applications] (
    [invoice] char(18)  NOT NULL,
    [expedition_date] datetime  NOT NULL,
    [minimun_amount_accepted] decimal(19,4)  NOT NULL,
    [purpose] nvarchar(300)  NOT NULL,
    [requested_amount] decimal(19,4)  NOT NULL,
    [client_rfc] varchar(13)  NOT NULL,
    [credit_invoice] char(18)  NOT NULL,
    [id_credit_application_state] int  NOT NULL,
    [id_dictum] int  NOT NULL,
    [employee_number] char(10)  NOT NULL,
    [credit_condition_identifier] nvarchar(6)  NOT NULL,
    [id_credit_type] int  NOT NULL
);
GO

-- Creating table 'credit_applications_state'
CREATE TABLE [dbo].[credit_applications_state] (
    [id_credit_application_state] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(10)  NOT NULL
);
GO

-- Creating table 'credit_conditions'
CREATE TABLE [dbo].[credit_conditions] (
    [identifier] nvarchar(6)  NOT NULL,
    [interest_rate] float  NOT NULL,
    [is_active] bit  NOT NULL,
    [is_iva_applied] bit  NOT NULL,
    [interest_on_arrears] float  NOT NULL,
    [advance_payment_reduction] float  NOT NULL,
    [payment_months] int  NOT NULL,
    [id_credit_type] int  NOT NULL
);
GO

-- Creating table 'credit_granting_polices'
CREATE TABLE [dbo].[credit_granting_polices] (
    [title] nvarchar(60)  NOT NULL,
    [effective_date] datetime  NOT NULL,
    [description] nvarchar(200)  NOT NULL,
    [is_active] bit  NOT NULL
);
GO

-- Creating table 'credit_types'
CREATE TABLE [dbo].[credit_types] (
    [id_credit_type] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(60)  NOT NULL
);
GO

-- Creating table 'credits'
CREATE TABLE [dbo].[credits] (
    [invoice] char(18)  NOT NULL,
    [approval_date] datetime  NOT NULL,
    [settlement_date] datetime  NOT NULL,
    [withdrawal_date] datetime  NOT NULL,
    [ammount_approved] decimal(19,4)  NOT NULL,
    [client_rfc] varchar(13)  NOT NULL,
    [credit_type_identifier] nchar(10)  NOT NULL,
    [id_credit_type] int  NOT NULL
);
GO

-- Creating table 'dictums'
CREATE TABLE [dbo].[dictums] (
    [id_dictum] int IDENTITY(1,1) NOT NULL,
    [credit_application_invoice] char(18)  NOT NULL,
    [employee_number] char(10)  NOT NULL
);
GO

-- Creating table 'digital_documents'
CREATE TABLE [dbo].[digital_documents] (
    [id_digital_document] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(30)  NOT NULL,
    [format] varchar(5)  NOT NULL,
    [contect] varbinary(max)  NOT NULL,
    [credit_application_invoice] char(18)  NOT NULL
);
GO

-- Creating table 'employee_roles'
CREATE TABLE [dbo].[employee_roles] (
    [id_employee_rol] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(60)  NOT NULL
);
GO

-- Creating table 'personal_references'
CREATE TABLE [dbo].[personal_references] (
    [ine_key] char(18)  NOT NULL,
    [name] nvarchar(60)  NOT NULL,
    [surname] nvarchar(60)  NULL,
    [last_name] nvarchar(60)  NOT NULL,
    [kinship] nvarchar(60)  NOT NULL,
    [relationship_years] varchar(2)  NOT NULL,
    [id_address] int  NOT NULL,
    [client_rfc] varchar(13)  NULL
);
GO

-- Creating table 'polices_apply_dictums'
CREATE TABLE [dbo].[polices_apply_dictums] (
    [id_polices_apply_dictums] int IDENTITY(1,1) NOT NULL,
    [id_dictum] int  NOT NULL,
    [credit_granting_police_title] nvarchar(60)  NOT NULL,
    [is_applied] bit  NOT NULL
);
GO

-- Creating table 'regimes'
CREATE TABLE [dbo].[regimes] (
    [id_regime] int IDENTITY(1,1) NOT NULL,
    [application_end_date] datetime  NOT NULL,
    [application_start_date] datetime  NOT NULL,
    [credit_conditions_identifier] nvarchar(6)  NOT NULL,
    [credit_invoice] char(18)  NOT NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'system_accounts'
CREATE TABLE [dbo].[system_accounts] (
    [employee_number] char(10)  NOT NULL,
    [name] nvarchar(60)  NOT NULL,
    [last_name] nvarchar(60)  NOT NULL,
    [surname] nvarchar(60)  NOT NULL,
    [id_employee_rol] int  NOT NULL,
    [password] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'work_centers'
CREATE TABLE [dbo].[work_centers] (
    [id_work_center] int IDENTITY(1,1) NOT NULL,
    [company_name] nvarchar(60)  NOT NULL,
    [employee_seniority] varchar(2)  NOT NULL,
    [human_resources_phone] varchar(15)  NOT NULL,
    [phone_number] varchar(15)  NOT NULL,
    [salary] decimal(19,4)  NOT NULL,
    [employee_position] nvarchar(60)  NOT NULL,
    [id_address] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id_address] in table 'addresses'
ALTER TABLE [dbo].[addresses]
ADD CONSTRAINT [PK_addresses]
    PRIMARY KEY CLUSTERED ([id_address] ASC);
GO

-- Creating primary key on [card_number] in table 'bank_accounts'
ALTER TABLE [dbo].[bank_accounts]
ADD CONSTRAINT [PK_bank_accounts]
    PRIMARY KEY CLUSTERED ([card_number] ASC);
GO

-- Creating primary key on [rfc] in table 'clients'
ALTER TABLE [dbo].[clients]
ADD CONSTRAINT [PK_clients]
    PRIMARY KEY CLUSTERED ([rfc] ASC);
GO

-- Creating primary key on [id_contact_method_type] in table 'contact_method_types'
ALTER TABLE [dbo].[contact_method_types]
ADD CONSTRAINT [PK_contact_method_types]
    PRIMARY KEY CLUSTERED ([id_contact_method_type] ASC);
GO

-- Creating primary key on [id_contact_method] in table 'contact_methods'
ALTER TABLE [dbo].[contact_methods]
ADD CONSTRAINT [PK_contact_methods]
    PRIMARY KEY CLUSTERED ([id_contact_method] ASC);
GO

-- Creating primary key on [invoice] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [PK_credit_applications]
    PRIMARY KEY CLUSTERED ([invoice] ASC);
GO

-- Creating primary key on [id_credit_application_state] in table 'credit_applications_state'
ALTER TABLE [dbo].[credit_applications_state]
ADD CONSTRAINT [PK_credit_applications_state]
    PRIMARY KEY CLUSTERED ([id_credit_application_state] ASC);
GO

-- Creating primary key on [identifier] in table 'credit_conditions'
ALTER TABLE [dbo].[credit_conditions]
ADD CONSTRAINT [PK_credit_conditions]
    PRIMARY KEY CLUSTERED ([identifier] ASC);
GO

-- Creating primary key on [title] in table 'credit_granting_polices'
ALTER TABLE [dbo].[credit_granting_polices]
ADD CONSTRAINT [PK_credit_granting_polices]
    PRIMARY KEY CLUSTERED ([title] ASC);
GO

-- Creating primary key on [id_credit_type] in table 'credit_types'
ALTER TABLE [dbo].[credit_types]
ADD CONSTRAINT [PK_credit_types]
    PRIMARY KEY CLUSTERED ([id_credit_type] ASC);
GO

-- Creating primary key on [invoice] in table 'credits'
ALTER TABLE [dbo].[credits]
ADD CONSTRAINT [PK_credits]
    PRIMARY KEY CLUSTERED ([invoice] ASC);
GO

-- Creating primary key on [id_dictum] in table 'dictums'
ALTER TABLE [dbo].[dictums]
ADD CONSTRAINT [PK_dictums]
    PRIMARY KEY CLUSTERED ([id_dictum] ASC);
GO

-- Creating primary key on [id_digital_document] in table 'digital_documents'
ALTER TABLE [dbo].[digital_documents]
ADD CONSTRAINT [PK_digital_documents]
    PRIMARY KEY CLUSTERED ([id_digital_document] ASC);
GO

-- Creating primary key on [id_employee_rol] in table 'employee_roles'
ALTER TABLE [dbo].[employee_roles]
ADD CONSTRAINT [PK_employee_roles]
    PRIMARY KEY CLUSTERED ([id_employee_rol] ASC);
GO

-- Creating primary key on [ine_key] in table 'personal_references'
ALTER TABLE [dbo].[personal_references]
ADD CONSTRAINT [PK_personal_references]
    PRIMARY KEY CLUSTERED ([ine_key] ASC);
GO

-- Creating primary key on [id_polices_apply_dictums] in table 'polices_apply_dictums'
ALTER TABLE [dbo].[polices_apply_dictums]
ADD CONSTRAINT [PK_polices_apply_dictums]
    PRIMARY KEY CLUSTERED ([id_polices_apply_dictums] ASC);
GO

-- Creating primary key on [id_regime] in table 'regimes'
ALTER TABLE [dbo].[regimes]
ADD CONSTRAINT [PK_regimes]
    PRIMARY KEY CLUSTERED ([id_regime] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [employee_number] in table 'system_accounts'
ALTER TABLE [dbo].[system_accounts]
ADD CONSTRAINT [PK_system_accounts]
    PRIMARY KEY CLUSTERED ([employee_number] ASC);
GO

-- Creating primary key on [id_work_center] in table 'work_centers'
ALTER TABLE [dbo].[work_centers]
ADD CONSTRAINT [PK_work_centers]
    PRIMARY KEY CLUSTERED ([id_work_center] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [id_address] in table 'clients'
ALTER TABLE [dbo].[clients]
ADD CONSTRAINT [FK_clients_addresses]
    FOREIGN KEY ([id_address])
    REFERENCES [dbo].[addresses]
        ([id_address])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_clients_addresses'
CREATE INDEX [IX_FK_clients_addresses]
ON [dbo].[clients]
    ([id_address]);
GO

-- Creating foreign key on [id_address] in table 'personal_references'
ALTER TABLE [dbo].[personal_references]
ADD CONSTRAINT [FK_personal_references_addresses]
    FOREIGN KEY ([id_address])
    REFERENCES [dbo].[addresses]
        ([id_address])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_personal_references_addresses'
CREATE INDEX [IX_FK_personal_references_addresses]
ON [dbo].[personal_references]
    ([id_address]);
GO

-- Creating foreign key on [id_address] in table 'work_centers'
ALTER TABLE [dbo].[work_centers]
ADD CONSTRAINT [FK_work_centers_addresses]
    FOREIGN KEY ([id_address])
    REFERENCES [dbo].[addresses]
        ([id_address])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_work_centers_addresses'
CREATE INDEX [IX_FK_work_centers_addresses]
ON [dbo].[work_centers]
    ([id_address]);
GO

-- Creating foreign key on [card_number] in table 'clients'
ALTER TABLE [dbo].[clients]
ADD CONSTRAINT [FK_clients_bank_accounts]
    FOREIGN KEY ([card_number])
    REFERENCES [dbo].[bank_accounts]
        ([card_number])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_clients_bank_accounts'
CREATE INDEX [IX_FK_clients_bank_accounts]
ON [dbo].[clients]
    ([card_number]);
GO

-- Creating foreign key on [id_work_center] in table 'clients'
ALTER TABLE [dbo].[clients]
ADD CONSTRAINT [FK_clients_work_centers]
    FOREIGN KEY ([id_work_center])
    REFERENCES [dbo].[work_centers]
        ([id_work_center])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_clients_work_centers'
CREATE INDEX [IX_FK_clients_work_centers]
ON [dbo].[clients]
    ([id_work_center]);
GO

-- Creating foreign key on [client_rfc] in table 'contact_methods'
ALTER TABLE [dbo].[contact_methods]
ADD CONSTRAINT [FK_contact_methods_clients]
    FOREIGN KEY ([client_rfc])
    REFERENCES [dbo].[clients]
        ([rfc])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_contact_methods_clients'
CREATE INDEX [IX_FK_contact_methods_clients]
ON [dbo].[contact_methods]
    ([client_rfc]);
GO

-- Creating foreign key on [client_rfc] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [FK_credit_applications_clients]
    FOREIGN KEY ([client_rfc])
    REFERENCES [dbo].[clients]
        ([rfc])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_applications_clients'
CREATE INDEX [IX_FK_credit_applications_clients]
ON [dbo].[credit_applications]
    ([client_rfc]);
GO

-- Creating foreign key on [client_rfc] in table 'credits'
ALTER TABLE [dbo].[credits]
ADD CONSTRAINT [FK_credits_clients]
    FOREIGN KEY ([client_rfc])
    REFERENCES [dbo].[clients]
        ([rfc])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credits_clients'
CREATE INDEX [IX_FK_credits_clients]
ON [dbo].[credits]
    ([client_rfc]);
GO

-- Creating foreign key on [client_rfc] in table 'personal_references'
ALTER TABLE [dbo].[personal_references]
ADD CONSTRAINT [FK_personal_references_clients]
    FOREIGN KEY ([client_rfc])
    REFERENCES [dbo].[clients]
        ([rfc])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_personal_references_clients'
CREATE INDEX [IX_FK_personal_references_clients]
ON [dbo].[personal_references]
    ([client_rfc]);
GO

-- Creating foreign key on [id_contact_method_type] in table 'contact_methods'
ALTER TABLE [dbo].[contact_methods]
ADD CONSTRAINT [FK_contact_methods_contact_method_types]
    FOREIGN KEY ([id_contact_method_type])
    REFERENCES [dbo].[contact_method_types]
        ([id_contact_method_type])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_contact_methods_contact_method_types'
CREATE INDEX [IX_FK_contact_methods_contact_method_types]
ON [dbo].[contact_methods]
    ([id_contact_method_type]);
GO

-- Creating foreign key on [ine_key] in table 'contact_methods'
ALTER TABLE [dbo].[contact_methods]
ADD CONSTRAINT [FK_contact_methods_personal_references]
    FOREIGN KEY ([ine_key])
    REFERENCES [dbo].[personal_references]
        ([ine_key])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_contact_methods_personal_references'
CREATE INDEX [IX_FK_contact_methods_personal_references]
ON [dbo].[contact_methods]
    ([ine_key]);
GO

-- Creating foreign key on [id_credit_application_state] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [FK_credit_applications_credit_applications_state]
    FOREIGN KEY ([id_credit_application_state])
    REFERENCES [dbo].[credit_applications_state]
        ([id_credit_application_state])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_applications_credit_applications_state'
CREATE INDEX [IX_FK_credit_applications_credit_applications_state]
ON [dbo].[credit_applications]
    ([id_credit_application_state]);
GO

-- Creating foreign key on [credit_condition_identifier] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [FK_credit_applications_credit_conditions]
    FOREIGN KEY ([credit_condition_identifier])
    REFERENCES [dbo].[credit_conditions]
        ([identifier])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_applications_credit_conditions'
CREATE INDEX [IX_FK_credit_applications_credit_conditions]
ON [dbo].[credit_applications]
    ([credit_condition_identifier]);
GO

-- Creating foreign key on [id_credit_type] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [FK_credit_applications_credit_types]
    FOREIGN KEY ([id_credit_type])
    REFERENCES [dbo].[credit_types]
        ([id_credit_type])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_applications_credit_types'
CREATE INDEX [IX_FK_credit_applications_credit_types]
ON [dbo].[credit_applications]
    ([id_credit_type]);
GO

-- Creating foreign key on [credit_invoice] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [FK_credit_applications_credits]
    FOREIGN KEY ([credit_invoice])
    REFERENCES [dbo].[credits]
        ([invoice])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_applications_credits'
CREATE INDEX [IX_FK_credit_applications_credits]
ON [dbo].[credit_applications]
    ([credit_invoice]);
GO

-- Creating foreign key on [id_dictum] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [FK_credit_applications_dictums]
    FOREIGN KEY ([id_dictum])
    REFERENCES [dbo].[dictums]
        ([id_dictum])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_applications_dictums'
CREATE INDEX [IX_FK_credit_applications_dictums]
ON [dbo].[credit_applications]
    ([id_dictum]);
GO

-- Creating foreign key on [employee_number] in table 'credit_applications'
ALTER TABLE [dbo].[credit_applications]
ADD CONSTRAINT [FK_credit_applications_system_accounts]
    FOREIGN KEY ([employee_number])
    REFERENCES [dbo].[system_accounts]
        ([employee_number])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_applications_system_accounts'
CREATE INDEX [IX_FK_credit_applications_system_accounts]
ON [dbo].[credit_applications]
    ([employee_number]);
GO

-- Creating foreign key on [credit_application_invoice] in table 'digital_documents'
ALTER TABLE [dbo].[digital_documents]
ADD CONSTRAINT [FK_digital_documents_credit_applications]
    FOREIGN KEY ([credit_application_invoice])
    REFERENCES [dbo].[credit_applications]
        ([invoice])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_digital_documents_credit_applications'
CREATE INDEX [IX_FK_digital_documents_credit_applications]
ON [dbo].[digital_documents]
    ([credit_application_invoice]);
GO

-- Creating foreign key on [id_credit_type] in table 'credit_conditions'
ALTER TABLE [dbo].[credit_conditions]
ADD CONSTRAINT [FK_credit_conditions_credit_types]
    FOREIGN KEY ([id_credit_type])
    REFERENCES [dbo].[credit_types]
        ([id_credit_type])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credit_conditions_credit_types'
CREATE INDEX [IX_FK_credit_conditions_credit_types]
ON [dbo].[credit_conditions]
    ([id_credit_type]);
GO

-- Creating foreign key on [credit_conditions_identifier] in table 'regimes'
ALTER TABLE [dbo].[regimes]
ADD CONSTRAINT [FK_regimes_credit_conditions]
    FOREIGN KEY ([credit_conditions_identifier])
    REFERENCES [dbo].[credit_conditions]
        ([identifier])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_regimes_credit_conditions'
CREATE INDEX [IX_FK_regimes_credit_conditions]
ON [dbo].[regimes]
    ([credit_conditions_identifier]);
GO

-- Creating foreign key on [credit_granting_police_title] in table 'polices_apply_dictums'
ALTER TABLE [dbo].[polices_apply_dictums]
ADD CONSTRAINT [FK_polices_apply_dictums_credit_granting_polices]
    FOREIGN KEY ([credit_granting_police_title])
    REFERENCES [dbo].[credit_granting_polices]
        ([title])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_polices_apply_dictums_credit_granting_polices'
CREATE INDEX [IX_FK_polices_apply_dictums_credit_granting_polices]
ON [dbo].[polices_apply_dictums]
    ([credit_granting_police_title]);
GO

-- Creating foreign key on [id_credit_type] in table 'credits'
ALTER TABLE [dbo].[credits]
ADD CONSTRAINT [FK_credits_credit_types]
    FOREIGN KEY ([id_credit_type])
    REFERENCES [dbo].[credit_types]
        ([id_credit_type])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_credits_credit_types'
CREATE INDEX [IX_FK_credits_credit_types]
ON [dbo].[credits]
    ([id_credit_type]);
GO

-- Creating foreign key on [credit_invoice] in table 'regimes'
ALTER TABLE [dbo].[regimes]
ADD CONSTRAINT [FK_regimes_credits]
    FOREIGN KEY ([credit_invoice])
    REFERENCES [dbo].[credits]
        ([invoice])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_regimes_credits'
CREATE INDEX [IX_FK_regimes_credits]
ON [dbo].[regimes]
    ([credit_invoice]);
GO

-- Creating foreign key on [employee_number] in table 'dictums'
ALTER TABLE [dbo].[dictums]
ADD CONSTRAINT [FK_dictums_system_accounts]
    FOREIGN KEY ([employee_number])
    REFERENCES [dbo].[system_accounts]
        ([employee_number])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dictums_system_accounts'
CREATE INDEX [IX_FK_dictums_system_accounts]
ON [dbo].[dictums]
    ([employee_number]);
GO

-- Creating foreign key on [id_dictum] in table 'polices_apply_dictums'
ALTER TABLE [dbo].[polices_apply_dictums]
ADD CONSTRAINT [FK_polices_apply_dictums_dictums]
    FOREIGN KEY ([id_dictum])
    REFERENCES [dbo].[dictums]
        ([id_dictum])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_polices_apply_dictums_dictums'
CREATE INDEX [IX_FK_polices_apply_dictums_dictums]
ON [dbo].[polices_apply_dictums]
    ([id_dictum]);
GO

-- Creating foreign key on [id_employee_rol] in table 'system_accounts'
ALTER TABLE [dbo].[system_accounts]
ADD CONSTRAINT [FK_system_accounts_employee_roles]
    FOREIGN KEY ([id_employee_rol])
    REFERENCES [dbo].[employee_roles]
        ([id_employee_rol])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_system_accounts_employee_roles'
CREATE INDEX [IX_FK_system_accounts_employee_roles]
ON [dbo].[system_accounts]
    ([id_employee_rol]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------