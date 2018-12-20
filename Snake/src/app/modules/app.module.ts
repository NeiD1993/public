import { AppComponent } from '../components/app.component/app.component';
import { AppRoutingModule } from './app-routing.module';
import { GameModule } from './game/game.module';
import { MainComponent } from '../components/main.component/main.component';
import { NgModule } from '@angular/core';
import { SettingsModule } from './settings.module';
import { SettingsService } from '../services/settings.service';

@NgModule({
  declarations: [AppComponent, MainComponent],
  imports: [AppRoutingModule, GameModule, SettingsModule],
  providers: [SettingsService],
  bootstrap: [AppComponent]
})
export class AppModule { }