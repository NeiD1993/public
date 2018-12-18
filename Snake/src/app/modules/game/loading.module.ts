import { DirectivesModule } from './directives.module';
import { LoadingComponent } from 'src/app/components/game/loading.component/classes/loading.component';
import { NgModule } from '@angular/core';

@NgModule({
    declarations: [LoadingComponent],
    imports: [DirectivesModule],
    exports: [LoadingComponent]
})
export class LoadingModule { }