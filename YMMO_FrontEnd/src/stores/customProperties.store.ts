import { ref } from 'vue'

export interface CustomProperty {
  id: string
  title: string
  type: string
  condition: string
  energyClass: string
  price: number
  surface: number
  address: string
  image?: string
  mainFeatures: string[]
  available: boolean
  availabilityDate: string
  rooms: number
  furnishing: string
  description: string
  yearBuilt: number
  latitude: number
  longitude: number
  agencyName: string
  agentName: string
  agentId: string
  pictures: { url: string }[]
}

const customProperties = ref<CustomProperty[]>([])

export function useCustomProperties() {
  function addProperty(p: CustomProperty) {
    customProperties.value.push(p)
  }

  function getAll(): CustomProperty[] {
    return customProperties.value
  }

  function getById(id: string): CustomProperty | undefined {
    return customProperties.value.find(p => p.id === id)
  }

  return { customProperties, addProperty, getAll, getById }
}
