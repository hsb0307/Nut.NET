CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_Email] ON [User] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_Username] ON [User] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [User] ([CustomerGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [Language] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Slug] ON [UrlRecord] ([Slug] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Custom_1] ON [UrlRecord] ([EntityId] ASC, [EntityName] ASC, [LanguageId] ASC, [IsActive] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StoreMapping_EntityId_EntityName] ON [StoreMapping] ([EntityId] ASC, [EntityName] ASC)
GO