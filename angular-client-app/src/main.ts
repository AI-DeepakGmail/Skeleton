import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { setGlobalInjector } from './app/core/injection/global-injector';

bootstrapApplication(App, appConfig)
  .then(appRef => {
    setGlobalInjector(appRef.injector); 
  })
  .catch((err) => console.error(err));
