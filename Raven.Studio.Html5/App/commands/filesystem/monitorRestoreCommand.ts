import deleteDocumentCommand = require("commands/deleteDocumentCommand");
import commandBase = require("commands/commandBase");
import database = require("models/database");
import getDocumentWithMetadataCommand = require("commands/getDocumentWithMetadataCommand");
import appUrl = require("common/appUrl");

class monitorRestoreCommand extends commandBase {
    private db: database = appUrl.getSystemDatabase();

    constructor(private parentPromise: JQueryDeferred<any>, private filesystemName: string, private updateRestoreStatus: (restoreStatusDto) => void) {
        super();
    }

    execute(): JQueryPromise<any> {
        new getDocumentWithMetadataCommand("Raven/FileSystem/Restore/Status/" + this.filesystemName, this.db)
            .execute()
            .fail((response: JQueryXHR) => {
                setTimeout(() => this.execute(), 1000);
            })
            .done((restoreStatus: restoreStatusDto)=> {
                this.updateRestoreStatus(restoreStatus);

                if (restoreStatus.State == "Running") {
                    setTimeout(() => this.execute(), 1000);
                } else {
                    if (restoreStatus.State == "Completed") {
                        this.reportSuccess("Filesystem was successfully restored!");
                        this.parentPromise.resolve();
                    } else {
                        this.reportError("Filesystem wasn't restored!");
                        this.parentPromise.reject();
                    }
                }
            });
        return this.parentPromise;
    }
}

export = monitorRestoreCommand;