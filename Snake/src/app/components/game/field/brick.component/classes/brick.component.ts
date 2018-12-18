import { $enum, EnumWrapper } from "ts-enum-util";
import { AnimationEvent } from "@angular/animations";
import { brickComponentAnimations } from "./brick.component.animations";
import { BrickComponentViewState } from "./brick.component.view-state";
import { BrickType } from "src/app/enums/brick-type";
import { BaseGameComponent } from "../../../base.game.component/base-game.components";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { ChangeDetectorRef, Component, EventEmitter } from "@angular/core";
import { FieldBrick } from "src/app/models/brick/field-brick";

@Component({
    selector: 'brick-root',
    styleUrls: ['../brick.component.css'],
    templateUrl: '../brick.component.html',
    animations: [brickComponentAnimations]
})
export class BrickComponent extends BaseGameComponent {

    constructor(gameLogicService: BaseGameLogicService, changeDetector: ChangeDetectorRef) {
        super(changeDetector, gameLogicService);
    }

    brick: FieldBrick;

    viewState: BrickComponentViewState;

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