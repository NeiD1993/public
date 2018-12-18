import { BrickComponent } from '../../components/game/field/brick.component/classes/brick.component';
import { BrickDecorationService } from '../../services/brick-decoration/brick-decoration.service';
import { IBrickDecorationServiceToken } from '../../services/brick-decoration/i-brick-decoration.service';
import { NgModule } from '@angular/core';

@NgModule({
    declarations: [BrickComponent],
    providers: [{ provide: IBrickDecorationServiceToken, useClass: BrickDecorationService }],
    exports: [BrickComponent],
    entryComponents: [BrickComponent]
})
export class BrickModule { }