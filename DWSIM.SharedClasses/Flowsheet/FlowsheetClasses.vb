﻿'    Main Form Auxiliary Classes
'    Copyright 2008 Daniel Wagner O. de Medeiros
'
'    This file is part of DWSIM.
'
'    DWSIM is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    DWSIM is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with DWSIM.  If not, see <http://www.gnu.org/licenses/>.

Imports System.Linq
Imports DWSIM.Interfaces
Imports DWSIM.Interfaces.Enums
Imports DWSIM.SharedClasses.Flowsheet

Namespace DWSIM.Flowsheet

    Public Enum MessageType
        Information
        Warning
        GeneralError
        Tip
    End Enum

    <System.Serializable()> Public Class ObjectCollection

        Public GraphicObjectCollection As Dictionary(Of String, IGraphicObject)

        Public FlowsheetObjectCollection As Dictionary(Of String, SharedClasses.UnitOperations.BaseClass)

        Public OPT_SensAnalysisCollection As List(Of Optimization.SensitivityAnalysisCase)

        Public OPT_OptimizationCollection As List(Of Optimization.OptimizationCase)

        Sub New()

            'Creates all the graphic collections.

            GraphicObjectCollection = New Dictionary(Of String, IGraphicObject)

            FlowsheetObjectCollection = New Dictionary(Of String, SharedClasses.UnitOperations.BaseClass)

            OPT_SensAnalysisCollection = New List(Of Optimization.SensitivityAnalysisCase)

            OPT_OptimizationCollection = New List(Of Optimization.OptimizationCase)

        End Sub

    End Class

    <System.Serializable()> Public Class FlowsheetVariables

        Implements Interfaces.ICustomXMLSerialization

        Implements Interfaces.IFlowsheetOptions

        Public AvailableUnitSystems As New Dictionary(Of String, SystemsOfUnits.Units)

        <Xml.Serialization.XmlIgnore()> Public PropertyPackages As Dictionary(Of String, IPropertyPackage)

        Public ReadOnly Property SelectedPropertyPackage() As IPropertyPackage
            Get
                For Each pp2 As IPropertyPackage In PropertyPackages.Values
                    Return pp2
                    Exit For
                Next
                Return Nothing
            End Get
        End Property

        Public SelectedComponents As Dictionary(Of String, Interfaces.ICompoundConstantProperties)

        Public NotSelectedComponents As Dictionary(Of String, Interfaces.ICompoundConstantProperties)

        Public Databases As Dictionary(Of String, String())

        Public Reactions As Dictionary(Of String, Interfaces.IReaction)

        Public ReactionSets As Dictionary(Of String, Interfaces.IReactionSet)

        Public Property SimulationMode As String = ""

        Public PetroleumAssays As Dictionary(Of String, Utilities.PetroleumCharacterization.Assay.Assay)

        Public SelectedUnitSystem As SystemsOfUnits.Units

        Sub New()

            SelectedComponents = New Dictionary(Of String, Interfaces.ICompoundConstantProperties)
            NotSelectedComponents = New Dictionary(Of String, Interfaces.ICompoundConstantProperties)
            SelectedUnitSystem = New SystemsOfUnits.SI()
            Reactions = New Dictionary(Of String, Interfaces.IReaction)
            ReactionSets = New Dictionary(Of String, Interfaces.IReactionSet)
            Databases = New Dictionary(Of String, String())
            PropertyPackages = New Dictionary(Of String, IPropertyPackage)
            PetroleumAssays = New Dictionary(Of String, Utilities.PetroleumCharacterization.Assay.Assay)

            'With ReactionSets
            '    .Add("DefaultSet", New ReactionSet("DefaultSet", DWSIM.App.GetLocalString("Rxn_DefaultSetName"), DWSIM.App.GetLocalString("Rxn_DefaultSetDesc")))
            'End With

        End Sub

        Public Function LoadData(data As System.Collections.Generic.List(Of System.Xml.Linq.XElement)) As Boolean Implements Interfaces.ICustomXMLSerialization.LoadData

            Dim el As XElement = (From xel As XElement In data Select xel Where xel.Name = "VisibleProperties").SingleOrDefault

            If Not el Is Nothing Then

                VisibleProperties.Clear()

                For Each xel2 As XElement In el.Elements
                    VisibleProperties.Add(xel2.@Value, New List(Of String))
                    For Each xel3 In xel2.Elements
                        VisibleProperties(xel2.@Value).Add(xel3.@Value)
                    Next
                Next

            End If

            Return XMLSerializer.XMLSerializer.Deserialize(Me, data)

        End Function

        Public Function SaveData() As System.Collections.Generic.List(Of System.Xml.Linq.XElement) Implements Interfaces.ICustomXMLSerialization.SaveData

            Dim elements As System.Collections.Generic.List(Of System.Xml.Linq.XElement) = XMLSerializer.XMLSerializer.Serialize(Me)

            elements.Add(New XElement("VisibleProperties"))

            For Each item In VisibleProperties
                Dim xel2 = New XElement("ObjectType", New XAttribute("Value", item.Key))
                elements(elements.Count - 1).Add(xel2)
                For Each item2 In item.Value
                    xel2.Add(New XElement("PropertyID", New XAttribute("Value", item2)))
                Next
            Next

            Return elements

        End Function

        Public Property BackupFileName As String = "" Implements Interfaces.IFlowsheetOptions.BackupFileName

        Public Property FilePath As String = "" Implements Interfaces.IFlowsheetOptions.FilePath

        Public Property FlowsheetQuickConnect As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetQuickConnect

        Public Property FlowsheetShowCalculationQueue As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetShowCalculationQueue

        Public Property FlowsheetShowConsoleWindow As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetShowConsoleWindow

        Public Property FlowsheetShowCOReportsWindow As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetShowCOReportsWindow

        Public Property FlowsheetShowWatchWindow As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetShowWatchWindow

        Public Property FlowsheetMultiSelectMode As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetMultiSelectMode

        Public Property FlowsheetSnapToGrid As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetSnapToGrid

        Public Property FlowsheetDisplayGrid As Boolean = False Implements Interfaces.IFlowsheetOptions.FlowsheetDisplayGrid

        Public Property FractionNumberFormat As String = "G8" Implements Interfaces.IFlowsheetOptions.FractionNumberFormat

        Public Property Key As String = "" Implements Interfaces.IFlowsheetOptions.Key

        Public Property NumberFormat As String = "G6" Implements Interfaces.IFlowsheetOptions.NumberFormat

        Public Property Password As String = "" Implements Interfaces.IFlowsheetOptions.Password

        Public Property SimulationAuthor As String = "" Implements Interfaces.IFlowsheetOptions.SimulationAuthor

        Public Property SimulationComments As String = "" Implements Interfaces.IFlowsheetOptions.SimulationComments

        Public Property SimulationName As String = "MySimulation_" & Date.Now.Second.ToString() Implements Interfaces.IFlowsheetOptions.SimulationName

        Public Property UsePassword As Boolean = False Implements Interfaces.IFlowsheetOptions.UsePassword

        Public Property SelectedUnitSystem1 As Interfaces.IUnitsOfMeasure Implements Interfaces.IFlowsheetOptions.SelectedUnitSystem
            Get
                Return Me.SelectedUnitSystem
            End Get
            Set(value As Interfaces.IUnitsOfMeasure)
                Me.SelectedUnitSystem = value
            End Set
        End Property

        Public Property VisibleProperties As New Dictionary(Of String, List(Of String)) Implements Interfaces.IFlowsheetOptions.VisibleProperties

        Public Property SimultaneousAdjustSolverEnabled As Boolean = True Implements Interfaces.IFlowsheetOptions.SimultaneousAdjustSolverEnabled

        Public Property SpreadsheetUseRegionalSeparator As Boolean = False Implements Interfaces.IFlowsheetOptions.SpreadsheetUseRegionalSeparator

        Public Property EnergyBalanceCheck As Enums.WarningType = Enums.WarningType.ShowWarning Implements IFlowsheetOptions.EnergyBalanceCheck

        Public Property MassBalanceCheck As Enums.WarningType = Enums.WarningType.ShowWarning Implements IFlowsheetOptions.MassBalanceCheck

        Public Property EnergyBalanceRelativeTolerance As Double = 0.01 Implements IFlowsheetOptions.EnergyBalanceRelativeTolerance

        Public Property MassBalanceRelativeTolerance As Double = 0.01 Implements IFlowsheetOptions.MassBalanceRelativeTolerance

        Public Property DisplayCornerPropertyList As Boolean = False Implements IFlowsheetOptions.DisplayCornerPropertyList

        Public Property DisplayCornerPropertyListPosition As Enums.ListPosition = Enums.ListPosition.RightBottom Implements IFlowsheetOptions.DisplayCornerPropertyListPosition

        Public Property DisplayFloatingPropertyTables As Boolean = True Implements IFlowsheetOptions.DisplayFloatingPropertyTables

        Public Property DisplayCornerPropertyListFontColor As String = "DimGray" Implements IFlowsheetOptions.DisplayCornerPropertyListFontColor

        Public Property DisplayCornerPropertyListFontName As String = "Consolas" Implements IFlowsheetOptions.DisplayCornerPropertyListFontName

        Public Property DisplayCornerPropertyListFontSize As Integer = 8 Implements IFlowsheetOptions.DisplayCornerPropertyListFontSize

        Public Property DisplayCornerPropertyListPadding As Integer = 4 Implements IFlowsheetOptions.DisplayCornerPropertyListPadding

        Public Property DefaultFloatingTableCompoundAmountBasis As CompositionBasis = CompositionBasis.Molar_Fractions Implements IFlowsheetOptions.DefaultFloatingTableCompoundAmountBasis

        Public Property DisplayFloatingTableCompoundAmounts As Boolean = True Implements IFlowsheetOptions.DisplayFloatingTableCompoundAmounts

        Public Property SpreadsheetUnitLockingMode As Boolean = True Implements IFlowsheetOptions.SpreadsheetUnitLockingMode

        Public Property CompoundOrderingMode As Enums.CompoundOrdering = CompoundOrdering.AsAdded Implements IFlowsheetOptions.CompoundOrderingMode

        Public Property FlowsheetControlPanelMode As Boolean = False Implements IFlowsheetOptions.FlowsheetControlPanelMode

        Public Property SkipEquilibriumCalculationOnDefinedStreams As Boolean = True Implements IFlowsheetOptions.SkipEquilibriumCalculationOnDefinedStreams

        Public Property ForceStreamPhase As ForcedPhase = ForcedPhase.None Implements IFlowsheetOptions.ForceStreamPhase

        Public Property DisplayUserDefinedPropertiesEditor As Boolean = False Implements IFlowsheetOptions.DisplayUserDefinedPropertiesEditor

    End Class

End Namespace
