// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Local } from 'protractor/built/driverProviders';
import { Injectable } from '@angular/core';
import { Observable, Subject, ReplaySubject, BehaviorSubject } from 'rxjs';

import { Point } from '../shared/point';
import { TrackingPoint } from '../shared/tracking-point';
import { Trip } from '../shared/trip';
import { Geofence } from '../shared/geofence';
import { Location } from '../shared/location';
import { Asset } from '../assets/asset';


@Injectable()
export class MapsService {

    private points = new ReplaySubject<{ points: Point[]; snappedPoints: boolean; }>();
    private trips = new ReplaySubject<Trip[]>();
    private trip = new ReplaySubject<Trip>();
    private geofence = new ReplaySubject<Geofence>();
    private geofences = new ReplaySubject<Geofence[]>();
    private currentDrawEnd = new ReplaySubject();
    private geofenceCircularRadiusChange = new ReplaySubject<number>();
    private geofenceCircularDraw = new ReplaySubject<[Point, number]>();
    private geofenceCircularDrawResult = new BehaviorSubject<Point>(null);
    private geofencePolygonDraw = new ReplaySubject<Point[]>();
    private geofencePolygonDrawResult = new BehaviorSubject<Point[]>(new Array<Point>());
    private assetsPositions = new ReplaySubject<[{ [key: string]: TrackingPoint }, boolean]>();
    private assetPosition = new ReplaySubject<[Asset, TrackingPoint]>();
    private devicesPositions = new ReplaySubject<[Map<string, TrackingPoint>, boolean]>();
    private devicePosition = new ReplaySubject<[string, TrackingPoint]>();
    private dispatchingPinsDraw = new ReplaySubject<Location[]>();
    private dispatchingPinsDrawResult = new BehaviorSubject<Location[]>(new Array<Location>());
    private dispatchingPoints = new ReplaySubject<Point[]>();
    private dispatchingAlternativePoints = new ReplaySubject<Point[]>();
    private dispatchingPins = new ReplaySubject<Location[]>();
    private locationsPositions = new ReplaySubject<Location[]>();
    private locationPosition = new ReplaySubject<Location>();
    private locationPinDraw = new ReplaySubject<Location>();
    private locationPinResult = new Subject<Location>();
    private itineraryPoint = new ReplaySubject<Point>();
    private locationSearchQuery = new ReplaySubject<string>();
    private locationSearchResult = new Subject<Point>();

    private routeColor: number;

    constructor() { }

    geocodeQuery(address: string) {
        this.locationSearchQuery.next(address);
    }

    geocodeResult(point: Point) {
        this.locationSearchResult.next(point);
    }

    showPoints(p: Point[], snapped = false) {
        this.points.next({points: p, snappedPoints: snapped});
    }

    showGeofence(geofence: Geofence) {
        this.geofence.next(geofence);
    }

    showGeofences(geofences: Geofence[]) {
        this.geofences.next(geofences);
    }

    startPolygonGeofenceDraw(initialPoints: Point[] = null) {
        this.geofencePolygonDraw.next(initialPoints);
    }

    startCircularGeofenceDraw(initialCenter: Point = null, radius: number) {
        this.geofenceCircularDraw.next([initialCenter, radius]);
    }

    changeCircularGeofenceRadius(radius: number) {
        this.geofenceCircularRadiusChange.next(radius);
    }

    showAssetsPositions(positions: { [key: string]: TrackingPoint }, inBackground = false) {
        this.assetsPositions.next([positions, inBackground]);
    }

    showAsset(position: [Asset, TrackingPoint]) {
        this.assetPosition.next(position);
    }

    showDevicesPositions(positions: Map<string, TrackingPoint>, inBackground = false) {
        this.devicesPositions.next([positions, inBackground]);
    }

    showDevice(position: [string, TrackingPoint]) {
        this.devicePosition.next(position);
    }

    showLocationsPositions(positions: Location[]) {
        this.locationsPositions.next(positions);
    }

    zoomToLocation(position: Location) {
        this.locationPosition.next(position);
    }

    startLocationPinDraw(location: Location) {
        this.locationPinDraw.next(location);
    }

    endCurrentDraw() {
        this.currentDrawEnd.next(null);
    }

    showTrips(trips: Trip[]) {
        this.trips.next(trips);
    }

    showTrip(trip: Trip) {
        this.trip.next(trip);
    }

    showDispatchingResults(points: Point[], pins: Location[]) {
        this.dispatchingPoints.next(points);
        this.dispatchingPins.next(pins);
    }

    showAlternativeResults(altPoints: Point[]) {
        this.dispatchingAlternativePoints.next(altPoints);
    }

    setRouteColor(color = 0) {
        this.routeColor = color;
    }

    startDispatchingPinsDraw(initialPoints: Location[] = null) {
        this.dispatchingPinsDraw.next(initialPoints);
        this.dispatchingPinsDrawResult.next([]);
    }

    showItineraryPosition(point: Point) {
        this.itineraryPoint.next(point);
    }

    getPoints(): Observable<{ points: Point[]; snappedPoints: boolean; }> {
        return this.points.asObservable();
    }

    getAssetsPositions(): Observable<[{ [key: string]: TrackingPoint }, boolean]> {
        return this.assetsPositions.asObservable();
    }

    getAssetPosition(): Observable<[Asset, TrackingPoint]> {
        return this.assetPosition.asObservable();
    }

    getDevicesPositions(): Observable<[Map<string, TrackingPoint>, boolean]> {
        return this.devicesPositions.asObservable();
    }

    getDevicePosition(): Observable<[string, TrackingPoint]> {
        return this.devicePosition.asObservable();
    }

    getLocationsPositions(): Observable<Location[]> {
        return this.locationsPositions.asObservable();
    }

    getLocationPosition(): Observable<Point> {
        return this.locationPosition.asObservable();
    }

    getLocationPinDraw(): Observable<Location> {
        return this.locationPinDraw.asObservable();
    }

    getLocationPinResult(): Observable<Location> {
        return this.locationPinResult.asObservable();
    }

    getLocationPinResultSubject(): Subject<Location> {
        return this.locationPinResult;
    }

    getDispatchingPinsDraw(): Observable<Location[]> {
        return this.dispatchingPinsDraw.asObservable();
    }

    getDispatchingPinsResult(): Observable<Location[]> {
        return this.dispatchingPinsDrawResult.asObservable();
    }

    getDispatchingPinsResultSubject(): Subject<Location[]> {
        return this.dispatchingPinsDrawResult;
    }
    getDispatchingPoints(): Observable<Point[]> {
        return this.dispatchingPoints.asObservable();
    }

    getRouteColor(): number {
        return this.routeColor;
    }

    getDispatchingPins(): Observable<Location[]> {
        return this.dispatchingPins.asObservable();
    }

    getDispatchingAltPoints(): Observable<Point[]> {
        return this.dispatchingAlternativePoints.asObservable();
    }

    getItineraryPosition(): Observable<Point> {
        return this.itineraryPoint.asObservable();
    }

    getTrip(): Observable<Trip> {
        return this.trip.asObservable();
    }

    getTrips(): Observable<Trip[]> {
        return this.trips.asObservable();
    }

    getGeofence(): Observable<Geofence> {
        return this.geofence.asObservable();
    }

    getGeofences(): Observable<Geofence[]> {
        return this.geofences.asObservable();
    }

    getGeofencePolygonDraw(): Observable<Point[]> {
        return this.geofencePolygonDraw.asObservable();
    }

    getGeofenceCircularDraw(): Observable<[Point, number]> {
        return this.geofenceCircularDraw.asObservable();
    }

    getCurrentDrawEnd(): Observable<{}> {
        return this.currentDrawEnd.asObservable();
    }

    getGeodencePolygonDrawResult(): Observable<Point[]> {
        return this.geofencePolygonDrawResult.asObservable();
    }

    getGeodenceCircularDrawResult(): Observable<Point> {
        return this.geofenceCircularDrawResult.asObservable();
    }

    getCircularGeofenceRadiusChange(): Observable<number> {
        return this.geofenceCircularRadiusChange.asObservable();
    }

    getGeofencePolygonDrawResultSubject(): Subject<Point[]> {
        return this.geofencePolygonDrawResult;
    }

    getGeofenceCircularDrawResultSubject(): Subject<Point> {
        return this.geofenceCircularDrawResult;
    }

    resetDispatchingDraw(initialPoints: Location[] = null) {
        this.endCurrentDraw();
        this.startDispatchingPinsDraw(initialPoints);
    }

    getGeocodeQuery(): Observable<string> {
        return this.locationSearchQuery.asObservable();
    }

    getGeocodeResult(): Observable<Point> {
        return this.locationSearchResult.asObservable();
    }

}
