// core/injection/global-injector.ts
import { EnvironmentInjector } from '@angular/core';

export let globalInjector: EnvironmentInjector;

export function setGlobalInjector(injector: EnvironmentInjector) {
  globalInjector = injector;
}
