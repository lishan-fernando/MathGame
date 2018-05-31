import { NgModule } from '@angular/core';
import { appRouting } from './app.routing';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';

@NgModule({
    imports: [BrowserModule, appRouting, FormsModule, ReactiveFormsModule, HttpModule],
    declarations: [AppComponent],
    bootstrap: [AppComponent]
})
export class AppModule { }
