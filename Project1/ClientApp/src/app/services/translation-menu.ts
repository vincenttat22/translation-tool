import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class TranslationMenu {
    back = {
        disabled : true,
    }
    forward = {
        disabled : true
    }
}