import { AnimationService } from 'src/app/services/animation.service';
import { AppRoutingModule } from '../app-routing.module';
import { BaseFieldBonusGeneratorService } from "src/app/services/generators/entity/bonus/base-field-bonus-generator.service";
import { BaseFieldGeneratorService } from '../../services/generators/field/base-field-generator.service';
import { BaseFoodGeneratorService } from '../../services/generators/entity/food/base-food-generator.service';
import { BaseSnakeGeneratorService } from '../../services/generators/entity/snake/base-snake-generator.service';
import { BrickOperationsExecutorService } from "src/app/services/brick-operations-executor.service";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DirectivesModule } from './directives.module';
import { FieldBonusGeneratorService } from "src/app/services/generators/entity/bonus/field-bonus-generator.service";
import { FieldGeneratorService } from '../../services/generators/field/field-generator.service';
import { FieldModule } from './field.module';
import { FoodGeneratorService } from '../../services/generators/entity/food/food-generator-service';
import { GameComponent } from '../../components/game/game.component/classes/game.component';
import { LoadingModule } from "./loading.module";
import { MathService } from "src/app/services/math.service";
import { NgModule } from '@angular/core';
import { SnakeDashboardModule } from './snake-dashboard.module';
import { SnakeGeneratorService } from '../../services/generators/entity/snake/snake-generator.service';

@NgModule({
  declarations: [GameComponent],
  imports: [AppRoutingModule, BrowserAnimationsModule, DirectivesModule, FieldModule, LoadingModule, SnakeDashboardModule],
  providers: [
    AnimationService,
    BrickOperationsExecutorService,
    { provide: BaseFieldBonusGeneratorService, useClass: FieldBonusGeneratorService },
    { provide: BaseFieldGeneratorService, useClass: FieldGeneratorService },
    { provide: BaseFoodGeneratorService, useClass: FoodGeneratorService },
    { provide: BaseSnakeGeneratorService, useClass: SnakeGeneratorService },
    MathService
  ]
})
export class GameModule { }