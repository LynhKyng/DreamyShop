import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppLayoutComponent } from './layout/app.layout.component';

const routes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./catalog/authentication/login/login.module').then(
        (m) => m.LoginModule
      ),
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./catalog/authentication/auth.module').then((m) => m.AuthModule),
  },
  {
    path: 'admin',
    pathMatch: 'full',
    loadChildren: () =>
      import('./catalog/admin-page/admin-page-routing.module').then(
        (m) => m.AdminPageRoutingModule
      ),
    component: AppLayoutComponent,
  },
  {
    path: 'product',
    pathMatch: 'full',
    loadChildren: () =>
      import('./catalog/product/product.module').then((m) => m.ProductModule),
    component: AppLayoutComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
