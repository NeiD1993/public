import { AppRoutingModule } from './app-routing.module';
import { DirectivesModule } from './game/directives.module';
import { NgModule } from '@angular/core';
import { SettingsComponent } from 'src/app/components/settings/settings.component/classes/settings.component';
import { SettingsOptionComponent } from '../components/settings/settings.option.component/classes/settings-option.component';

@NgModule({
    declarations: [SettingsComponent, SettingsOptionComponent],
    imports: [AppRoutingModule, DirectivesModule],
    exports: [SettingsComponent]
})
export class SettingsModule { }