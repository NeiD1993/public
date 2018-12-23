import { $enum, EnumWrapper } from "ts-enum-util";
import { AnimationEvent } from "@angular/animations";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { BaseViewStateComponent } from "../../../base.game.component/base-game.components";
import { brickComponentAnimations } from "./brick.component.animations";
import { BrickComponentViewState } from "./brick.component.view-state";
import { BrickType } from "src/app/enums/brick-type";
import { ChangeDetectorRef, Component, EventEmitter, Inject } from "@angular/core";
import { FieldBrick } from "src/app/models/brick/field-brick";
import { IBrickDecorationService, IBrickDecorationServiceToken } from "src/app/services/brick-decoration/i-brick-decoration.service";

@Component({
    selector: 'brick-root',
    styleUrls: ['../brick.component.css'],
    templateUrl: '../brick.component.html',
    animations: [brickComponentAnimations]
})
export class BrickComponent extends BaseViewStateComponent<BrickComponentViewState> {

    constructor(gameLogicService: BaseGameLogicService, changeDetector: ChangeDetectorRef,
        @Inject(IBrickDecorationServiceToken) private _bricksDecorationService: IBrickDecorationService) {
        super(changeDetector, gameLogicService, false);
    }

    brick: FieldBrick;

    protected createViewState(): BrickComponentViewState {
        return new BrickComponentViewState(this._bricksDecorationService, this.gameLogicService.difficulty);
    }

    onBrickContainerTriggerDone(animationEvent: AnimationEvent): void {
        let brickTypeWrapper: EnumWrapper = $enum(BrickType);
        let fromBrickType: BrickType = brickTypeWrapper.getValueOrDefault(animationEvent.fromState);
        let toBrickType: BrickType = brickTypeWrapper.getValueOrDefault(animationEvent.toState);

        if (((fromBrickType == BrickType.Snake) && (toBrickType == BrickType.Empty)) ||
            ((fromBrickType == BrickType.Empty) && (toBrickType == BrickType.Snake)))
            this.movingAnimationPerformed.next();
    }

    movingAnimationPerformed: EventEmitter<any> = new EventEmitter();
}