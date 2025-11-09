CREATE TABLE PartnerTypes (
    Id INT PRIMARY KEY,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DateCreated DATETIME NULL DEFAULT GETUTCDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL,
    Name NVARCHAR(50) NOT NULL
);

INSERT INTO PartnerTypes (Id, Name, DateCreated) 
VALUES (1, 'Personal', GETUTCDATE()), (2, 'Legal', GETUTCDATE());

CREATE TABLE Partners (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IsDeleted BIT NOT NULL DEFAULT 0,
    DateCreated DATETIME NULL DEFAULT GETUTCDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL,
    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(500) NULL,
    PartnerNumber NVARCHAR(20) NOT NULL,
    CroatianPIN NVARCHAR(11) NULL,
    PartnerTypeId INT NOT NULL,
    CreatedByUser NVARCHAR(255) NOT NULL,
    IsForeign BIT NOT NULL,
    ExternalCode NVARCHAR(20) NULL UNIQUE,
    Gender CHAR(1) NOT NULL CHECK (Gender IN ('M', 'F', 'N')),
    CONSTRAINT FK_Partners_PartnerTypes FOREIGN KEY (PartnerTypeId) 
        REFERENCES PartnerTypes(Id)
);

CREATE TABLE Policies (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IsDeleted BIT NOT NULL DEFAULT 0,
    DateCreated DATETIME NULL DEFAULT GETUTCDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL,
    PartnerId INT NOT NULL,
    PolicyNumber NVARCHAR(15) NOT NULL,
    PolicyAmount DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Policies_Partners FOREIGN KEY (PartnerId) 
        REFERENCES Partners(Id)
);

CREATE INDEX IX_Partners_DateCreated ON Partners(DateCreated DESC);
CREATE INDEX IX_Partners_PartnerNumber ON Partners(PartnerNumber);
CREATE INDEX IX_Policies_PartnerId ON Policies(PartnerId);