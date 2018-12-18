import { AnimationMetadata } from "@angular/animations";
import { BaseAnimationServiceGameComponentViewState } from "../../../base.game.component/base-animation-service-game.component.view-state";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { FoodStatus } from "src/app/enums/food-status";
import { GameState } from "src/app/enums/game/game-state";
import { snakeLengthDashboardComponentEatenActionIndicatorAnimation, snakeLengthDashboardComponentFreshActionIndicatorAnimation } from "./snake-length-dashboard.component.animations";

export class SnakeLengthDashboardComponentViewState extends BaseAnimationServiceGameComponentViewState {

    protected createAnimations(): Array<AnimationMetadata> {
        return [
            snakeLengthDashboardComponentEatenActionIndicatorAnimation,
            snakeLengthDashboardComponentFreshActionIndicatorAnimation
        ];
    }

    getActionIndicatorAnimation(foodStatus: FoodStatus): AnimationMetadata {
        return (foodStatus == FoodStatus.Eaten) ? snakeLengthDashboardComponentEatenActionIndicatorAnimation :
            snakeLengthDashboardComponentFreshActionIndicatorAnimation;
    }

    getFoodCountNumberType(isExtendedType: boolean, gameLogicService: BaseGameLogicService): string {
        if (isExtendedType && (gameLogicService.gameState == GameState.NotStarted))
            return FoodCountNumberType[FoodCountNumberType.Initial];
        else
            return ((gameLogicService.foodCount % 2 == 0) ? FoodCountNumberType[FoodCountNumberType.Even] : FoodCountNumberType[FoodCountNumberType.Odd]);
    }

    getLimiterBackgroundColor(foodStatus: FoodStatus): string {
        return (foodStatus == FoodStatus.Eaten) ? LengthDashboardBackgroundColor[LengthDashboardBackgroundColor.Black] :
            LengthDashboardBackgroundColor[LengthDashboardBackgroundColor.Green];
    }

    getSliderBackgroundColor(gameLogicService: BaseGameLogicService): string {
        return (this.getFoodCountNumberType(false, gameLogicService) == FoodCountNumberType[FoodCountNumberType.Even]) ?
            LengthDashboardBackgroundColor[LengthDashboardBackgroundColor.Blue] : LengthDashboardBackgroundColor[LengthDashboardBackgroundColor.Red];
    }
}

enum FoodCountNumberType {

    Even,

    Initial,

    Odd
}

enum LengthDashboardBackgroundColor {

    Black,

    Blue,

    Green,

    Red
}