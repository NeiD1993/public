
import { AnimatedAfterNgIfDirective } from 'src/app/directives/animated-ngIf/animated-after-ngIf.directive';
import { AnimatedBeforeNgIfDirective } from 'src/app/directives/animated-ngIf/animated-before-ngIf.directive';
import { AnimatedColorMapDirective } from 'src/app/directives/color-map/animated-color-map.directive';
import { FieldComponentIntervalForeachDirective } from 'src/app/components/game/field/field.component/classes/field.component.interval-foreach.directive';
import { NgModule } from '@angular/core';
import { NonAnimatedColorMapDirective } from 'src/app/directives/color-map/non-animated-color-map.directive';

let directives = [
    AnimatedAfterNgIfDirective,
    AnimatedBeforeNgIfDirective,
    AnimatedColorMapDirective,
    FieldComponentIntervalForeachDirective,
    NonAnimatedColorMapDirective
];

@NgModule({
    declarations: directives,
    exports: directives
})
export class DirectivesModule { }