
const images = import.meta.glob('@ui/assets/images/*.png', { eager: true, import: 'default' })

const DeviceImage = ({ name, className = 'device-image', alt = name }) => {
  const imagePath = `/src/ui/assets/images/${name}.png`
  const src = images[imagePath]
  const fallbackSrc = `/src/ui/assets/images/device-fallback.png`

  if (!src) {
    return <img className={className} src={fallbackSrc} alt={alt} />
  }

  return <img className={className} src={src} alt={alt} />
}
export default DeviceImage
