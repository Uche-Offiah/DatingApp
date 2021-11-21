import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(
    component: MemberEditComponent): boolean {
      //this guard prevents a user from leaving the Edit componet/page without comfirmation
      if (component.editFrom.dirty) {
        return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
      }
    return true;
  }
}
