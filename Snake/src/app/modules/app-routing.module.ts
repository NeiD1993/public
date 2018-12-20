import { GameComponent } from '../components/game/game.component/classes/game.component';
import { MainComponent } from '../components/main.component/main.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SettingsComponent } from '../components/settings/settings.component/classes/settings.component';

const routes: Routes = [
  { path: 'game', component: GameComponent },
  { path: 'main', component: MainComponent },
  { path: 'settings', component: SettingsComponent },
  { path: '**', redirectTo: 'main' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }