# RFID Tray Management System

## Introduction

This project is a tray management system that uses RFID technology to track the location of trays in a hospital. The system is designed to be used in a hospital setting to track the location of trays as they move through the hospital. The system consists of a set of RFID readers that are placed at various locations throughout the hospital, and a central server that collects and processes the data from the readers.

## Setup

### Hardware
1. Connect the RFID reader to the network
2. Make sure the web server on the reader is running (Turn it on in TS Service App)
3. Connect the antenna to the reader
4. Connect the camera to the network

### Web Interface

1. Install Docker
2. Install VSCode and Remote - Containers extension
3. Clone the repository
4. Open the repository in VSCode
5. Click on the green button in the bottom left corner of the window and select "Reopen in Container"
6. Run the following command to start the server:

```bash
./script.sh # sudo if necessary
```

## Usage

### First time configuration

1. Open the web interface at `http://localhost:3000`
2. Login with the default credentials
3. Go to the Config/Host IP page and set the local IP address of the reader
![Pictures/Screenshot from 2024-07-16 16-22-53.png](<Pictures/Screenshot from 2024-07-16 16-22-53.png>)
4. Go to the Config/Reader page and set the reader IP address and the function for each antenna port
![Config/Reader](<Pictures/Screenshot from 2024-07-16 16-22-44.png>)
5. Go to the Config/Power Level and overwrite the default power level if necessary
![Config/Power Level](<Pictures/Screenshot from 2024-07-16 16-22-59.png>)

### Normal operation

#### Creating a new rack

1. Go to the Rack Management page
![Rack Management](<Pictures/Screenshot from 2024-07-16 16-23-45.png>)
2. Click on the "Add Rack" button
3. Enter the rack name and the location
![Creat Rack](<Pictures/Screenshot from 2024-07-16 16-23-52.png>)
4. Click on the "Save" button

#### Creating a new tray

1. Go to the Tray Management/Create Tray page
![Create Tray](<Pictures/Screenshot from 2024-07-16 16-23-38.png>)
2. Put the tray in front of the ReadWrite antenna that is set to "ReadWrite" function in the reader configuration
3. Click on the "Scan" button
4. Select the rack where the tray is located
5. Click on the "Create" button

#### Reading a tray

1. Go to the Tray Management/Read Tray page
![Read Tray](<Pictures/Screenshot from 2024-07-16 16-23-32.png>)
2. Put the tray in front of the ReadWrite antenna that is set to "ReadWrite" function in the reader configuration
3. Click on the "Scan" button
4. The tray information will be displayed

#### Editing a tray

1. Go to the Tray Management page
![Tray Management](<Pictures/Screenshot from 2024-07-16 16-23-17.png>)
2. Click the three dots button on the tray you want to edit
3. Click on the "Edit" button
4. Edit the tray information
5. Put the tray in front of the ReadWrite antenna that is set to "ReadWrite" function in the reader configuration
6. Click on the "Save" button

#### Starting and stopping inventory

1. Click the "Start Inventory" button on the navigation bar to start inventory
2. Click the "Stop Inventory" button on the navigation bar to stop inventory

# Project Gantt Chart

```mermaid
gantt
    dateFormat  YYYY-MM-DD
    title       Sprint 1. Requirements gathering & migration
    axisFormat  %a %d/%m/%Y

    section Planning & Requirements Gathering
    Requirement Gathering & lit review                         :des1, 2024-09-02, 1d
    Requirements Engineering                                   :des2, 2024-09-02, 1d
    Plan & re-design distributed system                        :des3, 2024-09-02, 3d
    Code refactor part i                                       :des4, after des3, 3d
    Test and bug fixes                                         :des6, after des3, 3d

    section Planning & Requirements Gathering
    Code refactor part ii                                       :des5, after des4, 5d
    System Migration containerization of software               :des6, after des4, 5d
    System Migration Database migration & Setup                 :des7, after des4, 5d
    Scalability implementation                                  :des9, after des4, 5d
    Test and bug fixes                                          :des8, after des4, 6d
    Sprint task logs                                            :des9, after des8, 1d
    A Gantt chart                                               :des10, after des8, 1d
    
```

```mermaid
gantt
    dateFormat  YYYY-MM-DD
    title       Sprint 2. Auto config & Local software
    axisFormat  %a %d/%m/%Y

    section Auto-Configuration Implementation
    Auto-config file structure                                :des1, 2024-09-22, 1d
    Auto-config settings implementation                       :des2, after des1, 2d
    Integrate into cloud system                               :des3, after des2, 3d
    Integrate into local system                               :des4, after des1, 2d
    Test and bug fixes                                        :des6, after des4, 3d
    Local computer software system (backend/frontend)         :des5, after des6, 2d
    MQTT protocol establishment - RFID reader                 :des9, after des5, 3d
    Web API endpoints establishment - Cloud                   :des10, after des9, 2d
    Integrate with auto-configuration                         :des11, after des10, 2d
    Test and bug fixes                                        :des12, after des11, 2d
    
```

```mermaid
gantt
    dateFormat  YYYY-MM-DD
    title       Sprint 3. Testing
    axisFormat  %a %d/%m/%Y

    section System integration and initial testing
    Integration between local system and cloud system                                     :des1, 2024-10-16, 3d
    Data synchronization between local system and cloud                                   :des2, after des1, 3d
    Initial system testing                                                                :des3, after des1, 3d

    section Scalability testing
    System testing - Introduce additional lab ecosystem                                   :des4, after des1, 3d
    System testing - Load balancing and performance                                       :des5, after des2, 3d

    section Unit Testing - Reliability testing
    Unit testing - individual components                                                  :des6, after des2, 3d
    Integration testing - Component cohesiveness & coupling                               :des7, after des6, 3d
    Unit testing - auto saves and auto configuration                                      :des8, after des6, 3d
    Unit testing - System reliability under load                                          :des9, after des6, 3d

    section Unit Testing - Scenario based testing
    Testing system reliability under scenarios (system shutdown, restart etc)             :des10, after des6, 3d
```

```mermaid
gantt
    dateFormat  YYYY-MM-DD
    title       Sprint 4. UAT & Final delivery
    axisFormat  %a %d/%m/%Y

    section User acceptance testing & performance configuration
    UAT testing - scenario based test cases                                       :des1, 2024-11-15, 3d
    Final system optimization, correction and bug fix                             :des2, after des1, 2d

    section Report, documentation and final delivery
    Final report                                                                :des3, after des2, 3d
    System documentation                                                        :des4, after des3, 3d
```



