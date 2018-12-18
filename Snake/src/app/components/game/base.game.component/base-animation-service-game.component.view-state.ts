import { ActivatableGameComponentViewState } from "./base-activatable-game.component/base-activatable-game.component.view-state";
import { AnimationMetadata } from "@angular/animations";
import { AnimationService } from "src/app/services/animation.service";

export abstract class BaseAnimationServiceGameComponentViewState extends ActivatableGameComponentViewState {

    protected abstract createAnimations(): Array<AnimationMetadata>;

    createAnimationFactories(animationService: AnimationService): void {
        let animations: Array<AnimationMetadata> = this.createAnimations();

        animations.forEach(animation => animationService.addFactory(animation));
    }
}