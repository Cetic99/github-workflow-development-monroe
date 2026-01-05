/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconCheckmark = ({ color = 'black', size = 'm' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="50"
        height="50"
        viewBox="0 0 50 50"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <rect x="6" y="5" width="40" height="40" rx="20" fill="#1D4A3E" />
        <path
          d="M33.3055 18.7556C32.7743 18.7556 32.218 18.9348 31.8117 19.341L24.0826 27.1535C23.5472 27.691 23.0097 27.591 22.5888 26.9597L20.5118 23.8347C19.8743 22.8764 18.543 22.6098 17.5888 23.2473C16.6347 23.8869 16.368 25.2202 17.0034 26.1764L19.0826 29.3014C20.9576 32.1223 24.616 32.4805 27.0055 30.0847L34.7993 22.3368C35.6097 21.5223 35.6097 20.1556 34.7993 19.341C34.393 18.9348 33.8367 18.7556 33.3055 18.7556Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconCheckmark
