import { BrickModule } from './brick.module';
import { DirectivesModule } from './directives.module';
import { FieldComponent } from '../../components/game/field/field.component/classes/field.component';
import { NgModule } from '@angular/core';

@NgModule({
    declarations: [FieldComponent],
    imports: [BrickModule, DirectivesModule],
    exports: [FieldComponent]
})
export class FieldModule { }