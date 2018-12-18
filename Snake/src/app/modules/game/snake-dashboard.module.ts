import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DirectivesModule } from './directives.module';
import { NgModule } from '@angular/core';
import { SnakeDirectionDashboardComponent } from '../../components/game/snake-dashboard/snake-direction-dashboard.component/classes/snake-direction-dashboard.component';
import { SnakeLengthDashboardComponent } from '../../components/game/snake-dashboard/snake-length-dashboard.component/classes/snake-length-dashboard.component';

@NgModule({
    declarations: [SnakeDirectionDashboardComponent, SnakeLengthDashboardComponent],
    imports: [BrowserAnimationsModule, DirectivesModule],
    exports: [SnakeDirectionDashboardComponent, SnakeLengthDashboardComponent]
})
export class SnakeDashboardModule { }