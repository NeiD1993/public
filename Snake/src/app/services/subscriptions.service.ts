import { InjectionToken } from "@angular/core";
import { Subscription } from "rxjs";

export class SubscriptionsService<T> {

    private _subscriptions: Map<T, Subscription> = new Map<T, Subscription>();

    set subscriptions(subscriptions: Map<T, Subscription>) {
        this._subscriptions.clear();
        subscriptions.forEach((subscription, key) => this._subscriptions.set(key, subscription));
    }

    get subscriptions(): Map<T, Subscription> {
        return this._subscriptions;
    }

    private deleteSubscription(key: T): void {
        let subscription: Subscription = this._subscriptions.get(key);

        subscription.unsubscribe();
        this._subscriptions.delete(key);
    }

    addSubscription(key: T, subscription: Subscription): void {
        this._subscriptions.set(key, subscription);
    }

    resetSubscriptions(): void {
        this._subscriptions.forEach(({ }, key) => this.deleteSubscription(key));
    }

    tryRemoveSubscription(key: T): boolean {
        if (this._subscriptions.has(key)) {
            this.deleteSubscription(key);

            return true;
        }
        else
            return false;
    }
}

export let createSubscriptionServiceFactoryProvider = <T>(injectionToken: InjectionToken<any>) => {
    return {
        provide: injectionToken,
        useFactory: () => new SubscriptionsService<T>()
    }
}