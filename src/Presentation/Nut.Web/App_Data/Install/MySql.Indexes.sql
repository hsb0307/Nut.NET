CREATE INDEX `IX_LocaleStringResource` on `LocaleStringResource` (`ResourceName` ASC, `LanguageId` ASC);

CREATE INDEX `IX_User_Email` ON `User` (`Email` ASC);

CREATE INDEX `IX_User_Username` ON `User` (`Username` ASC);

CREATE INDEX `IX_User_UserGuid` ON `User` (`UserGuid` ASC);

CREATE INDEX `IX_Language_DisplayOrder` ON `Language` (`DisplayOrder` ASC);

CREATE INDEX `IX_ActivityLog_CreatedOnUtc` ON `ActivityLog` (`CreatedOnUtc` ASC);

CREATE INDEX `IX_UrlRecord_Slug` ON `UrlRecord` (`Slug` ASC);

CREATE INDEX `IX_UrlRecord_Custom_1` ON `UrlRecord` (`EntityId` ASC, `EntityName` ASC, `LanguageId` ASC, `IsActive` ASC);

CREATE INDEX `IX_StoreMapping_EntityId_EntityName` ON `StoreMapping` (`EntityId` ASC, `EntityName` ASC);
