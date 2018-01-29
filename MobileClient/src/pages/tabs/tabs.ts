import { Component } from '@angular/core';
import { SignallingService } from '../../providers/signalling-service';

import { HomePage } from '../home/home';
import { DebugPage } from '../debug/debug';
import { SettingsPage } from '../settings/settings';

@Component({
  templateUrl: 'tabs.html'
})
export class TabsPage {

  private tabs: Tab[] = [
    { page: HomePage, title: "Home", icon: "home" },
    { page: SettingsPage, title: "Settings", icon: "settings" }
  ];

  private settingsTabAdded = false;

  constructor(private signallingService: SignallingService) {
    this.signallingService.showDebugTabPromise.then(() => this.addSettingsTab());
  }

  addSettingsTab() {
    if (!this.settingsTabAdded) {
      this.tabs.push({ page: DebugPage, title: "Debug", icon: "bug" });
      this.settingsTabAdded = true;
    }
  }
}
