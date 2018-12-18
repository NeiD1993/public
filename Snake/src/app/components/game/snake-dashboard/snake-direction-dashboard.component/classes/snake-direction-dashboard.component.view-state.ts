import { AnimationMetadata } from "@angular/animations";
import { BaseAnimationServiceGameComponentViewState } from "../../../base.game.component/base-animation-service-game.component.view-state";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { BonusType } from "src/app/enums/bonus-type";
import { Snake } from "src/app/models/snake/snake";
import { snakeDirectionSnakeSpeedLevelTextChangedAnimations } from "./snake-direction-dashboard.component.animations"

export class SnakeDirectionDashboardComponentViewState extends BaseAnimationServiceGameComponentViewState {

    private _speedLevelTextChangedAnimationBuffer: AnimationMetadata;

    protected createAnimations(): AnimationMetadata[] {
        let array = Array.from(snakeDirectionSnakeSpeedLevelTextChangedAnimations.values());
        return array;
    }

    get speedLevelTextChangedAnimation(): AnimationMetadata {
        return this._speedLevelTextChangedAnimationBuffer;
    }

    getSpeedLevelTextContainerText(gameLogicService: BaseGameLogicService): string {
        let snakeSpeedLevel: number = gameLogicService.field.snake.speedLevel;

        switch (snakeSpeedLevel) {
            case 1:
                return snakeSpeedLevel + ' [Min]';
            case Snake.speedLevelsCount:
                return snakeSpeedLevel + ' [Max]';
            default:
                return snakeSpeedLevel.toString();
        }
    }

    getSpeedLevelTextContainerTextColorsCount(): number {
        return Snake.speedLevelsCount;
    }

    setSpeedLevelTextChangedAnimation(bonusType: BonusType) {
        this._speedLevelTextChangedAnimationBuffer = snakeDirectionSnakeSpeedLevelTextChangedAnimations.get(bonusType);
    }
}