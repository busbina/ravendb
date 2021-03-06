import commandBase = require("commands/commandBase");
import resource = require("models/resource");

class getSingleAuthTokenCommand extends commandBase {

    constructor(private resource: resource, private checkIfMachineAdmin :boolean = false) {
        super();

        if (this.resource == null) {
            throw new Error("Must specify resource");
        }
    }

    execute(): JQueryPromise<singleAuthToken> {
        var url = "/singleAuthToken";
        var args = null;

        if (this.checkIfMachineAdmin) {
            args = {
                CheckIfMachineAdmin:true
            };
        }
            
        var getTask = this.query(url, args, this.resource);

        return getTask;
    }
}

export = getSingleAuthTokenCommand;