import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './authentication/auth.guard';
import { AvatarModule } from 'ngx-avatar';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthInterceptor } from './authentication/auth.interceptor';
import { ApiService } from './services/api.service';
import { TranslationToolComponent } from './translation-tool/translation-tool.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTableModule, MatProgressBarModule, MatTreeModule, MatIconModule, MatCheckboxModule, MatDialogModule, MatFormFieldModule, MatSelectModule, MatButtonModule, MatExpansionModule, MatProgressSpinnerModule, MatTabsModule, MatInputModule, MatSlideToggleModule, MatCardModule, MatListModule, MatSnackBarModule } from '@angular/material';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TranslationConfigComponent } from './translation-tool/translation-config/translation-config.component';
import { TranslationProcessComponent } from './translation-tool/translation-process/translation-process.component';
import { UserComponent } from './user/user.component';
import { UserListComponent } from './user/user-list/user-list.component';
import { AddRoleDialog, UserRolesComponent } from './user/user-roles/user-roles.component';
import { AddEditUserComponent } from './user/user-list/add-edit-user/add-edit-user.component';
import { ConfirmationDialog } from './confirmation-dialog/confirmation-dialog.component';

@NgModule({
  entryComponents: [
    TranslationConfigComponent,
    TranslationProcessComponent,
    AddEditUserComponent,
    AddRoleDialog,
    ConfirmationDialog],
  declarations: [				
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginComponent,
    UserComponent,
    UserListComponent,
    UserRolesComponent,
    TranslationToolComponent,
    TranslationConfigComponent,
    TranslationProcessComponent,
    AddEditUserComponent,
    AddRoleDialog,
    ConfirmationDialog
   ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AvatarModule,
    NgbModule,
    MatTableModule,
    MatProgressBarModule,
    MatTreeModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatSelectModule,
    MatExpansionModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatIconModule,
    MatSlideToggleModule,
    MatListModule,
    FontAwesomeModule,
    MatButtonModule,
    MatSnackBarModule,
    MatCardModule,
    RouterModule.forRoot([
      {
        path: "",
        redirectTo: "/home",
        pathMatch: "full"
      },
      {
        path: "home",
        component: HomeComponent,
        canActivate: [AuthGuard]
      },
      {
        path: "user",
        component: UserComponent,
        canActivate: [AuthGuard]
      },
      {
        path: "translation",
        component: TranslationToolComponent,
        canActivate: [AuthGuard]
      },
      {
        path: "login",
        component: LoginComponent
      }
    ]),
    BrowserAnimationsModule
  ],
  providers: [ApiService,{
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
