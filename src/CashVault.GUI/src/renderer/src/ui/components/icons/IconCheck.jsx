/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconCheck = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="46"
        height="46"
        viewBox="0 0 46 46"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M22.9999 7.66665C14.5316 7.66665 7.66659 14.5316 7.66659 23C7.66659 31.4683 14.5316 38.3333 22.9999 38.3333C31.4683 38.3333 38.3333 31.4683 38.3333 23C38.3333 14.5316 31.4683 7.66665 22.9999 7.66665ZM3.83325 23C3.83325 12.4145 12.4145 3.83331 22.9999 3.83331C33.5854 3.83331 42.1666 12.4145 42.1666 23C42.1666 33.5854 33.5854 42.1666 22.9999 42.1666C12.4145 42.1666 3.83325 33.5854 3.83325 23Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M30.1052 17.8114C30.8537 18.5599 30.8537 19.7734 30.1052 20.522L22.4385 28.1886C21.69 28.9371 20.4765 28.9371 19.728 28.1886L15.8946 24.3553C15.1461 23.6068 15.1461 22.3932 15.8946 21.6447C16.6431 20.8962 17.8567 20.8962 18.6052 21.6447L21.0833 24.1228L27.3946 17.8114C28.1431 17.0629 29.3567 17.0629 30.1052 17.8114Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconCheck
