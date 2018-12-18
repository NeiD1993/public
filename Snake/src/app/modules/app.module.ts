import { AppComponent } from '../components/app.component/app.component';
import { AppRoutingModule } from './app-routing.module';
import { GameModule } from './game/game.module';
import { MainComponent } from '../components/main.component/main.component';
import { NgModule } from '@angular/core';

@NgModule({
  declarations: [AppComponent, MainComponent],
  imports: [AppRoutingModule, GameModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
