import { ApplicationConfig, provideBrowserGlobalErrorListeners, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import { FormsModule } from '@angular/forms';

// PrimeNG Theme
import Aura from '@primeuix/themes/aura';

// Routes
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [

    // ✅ Routing
    provideRouter(routes),

    // ✅ HTTP Client (API calls)
    provideHttpClient(withFetch()),

    importProvidersFrom(FormsModule),

    // ✅ Animations (required for PrimeNG)
    provideAnimationsAsync(),

    // ✅ PrimeNG Theme Setup
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: 'none'
        }
      }
    })

  ]
};