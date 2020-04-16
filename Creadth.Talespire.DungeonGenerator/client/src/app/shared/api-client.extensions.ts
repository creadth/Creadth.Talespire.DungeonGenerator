import {Injector} from '@angular/core';
import {of} from 'rxjs';
class ServiceBase {
    constructor(injector: Injector) {
    }

    protected getBaseUrl(token: string): string{
        return "";
    }

    protected transformOptions(options: {headers: any}): any{
        options.headers = options.headers.append('Cache-Control', 'no-cache');
        options.headers = options.headers.append('Pragma', 'no-cache');
        options.headers = options.headers.append('Expires', 'Sat, 01 Jan 2000 00:00:00 GMT');
        return of(options);
    }
}
