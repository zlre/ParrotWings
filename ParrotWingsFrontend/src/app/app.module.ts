import { AppRoutingModule } from './app-routing.module';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA  } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';

import { JwtInterceptor, ErrorInterceptor, EnumNamePipe } from './helpers';
import { LoginComponent } from './login';
import { TransactionsHistoryComponent, TransactionsCreateComponent } from './transactions';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { NgPipesModule } from 'ngx-pipes';
import { ClickableHeadComponent } from './clickableHead';
import { UserInfoComponent } from './user-info';


@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        ClickableHeadComponent,
        TransactionsHistoryComponent,
        TransactionsCreateComponent,
        UserInfoComponent,
        EnumNamePipe
    ],
    imports: [
        BrowserModule,
        ReactiveFormsModule,
        HttpClientModule,
        AppRoutingModule,
        FormsModule,
        BrowserAnimationsModule,
        ToastrModule.forRoot({
            timeOut: 2000,
            positionClass: 'toast-bottom-right',
            preventDuplicates: false,
        }),
        NgPipesModule
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },

    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
