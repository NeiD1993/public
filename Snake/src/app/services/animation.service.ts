import { AnimationBuilder, AnimationFactory, AnimationMetadata, AnimationPlayer } from "@angular/animations";
import { Injectable } from "@angular/core";

@Injectable()
export class AnimationService {

    private _animationFactories: Map<AnimationMetadata, AnimationFactory> = new Map<AnimationMetadata, AnimationFactory>();

    constructor(private _animationBuilder: AnimationBuilder) { }

    addFactory(animation: AnimationMetadata): void {
        if (!this._animationFactories.get(animation))
            this._animationFactories.set(animation, this._animationBuilder.build(animation));
    }

    deleteFactory(animation: AnimationMetadata): boolean {
        return (this._animationFactories.get(animation)) ? this._animationFactories.delete(animation) : false;
    }

    playAnimation(animation: AnimationMetadata, element: any, onDone?: () => void): void {
        let animationFactory: AnimationFactory = this._animationFactories.get(animation);

        if (animationFactory) {
            let animationPlayer: AnimationPlayer = animationFactory.create(element);

            if (onDone)
                animationPlayer.onDone(onDone);

            animationPlayer.play();
        }
    }
}